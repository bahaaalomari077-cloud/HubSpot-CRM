import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  CdkDrag, CdkDropList, CdkDropListGroup, CdkDragDrop, CdkDragPlaceholder,
  moveItemInArray, transferArrayItem
} from '@angular/cdk/drag-drop';

import { ContactService, BulkImportRow } from '../contacts/services/contact.service';
import { ActivityService, ACTIVITY_TYPE } from '../activities/services/activity.service';
import { MessageService, CHANNEL } from '../messages/services/message.service';
import {
  QuickActionModal, QuickActionKind, QuickActionPayload
} from '../../shared/components/quick-action/quick-action-modal';
import { Contact } from '../../core/models/entities.model';
import { ContactStatus } from '../../core/models/common.model';

/** Fixed board columns. `key` matches the API's camelCase grouping key. */
interface Column {
  key: string;
  status: ContactStatus;
  label: string;
  color: string;
}

/** Splits pasted text into lines. */
const NEWLINE_RE = /\r?\n/;
/** Accepts |, tab, comma or semicolon between fields. */
const SEPARATOR_RE = /\s*[|\t,;]\s*/;

const COLUMNS: Column[] = [
  { key: 'potential',  status: 'Potential',  label: 'Suggested',  color: '#64748b' }, // gray
  { key: 'contacted',  status: 'Contacted',  label: 'Contacted',  color: '#2f6df6' }, // blue
  { key: 'interested', status: 'Interested', label: 'Interested', color: '#d97706' }, // yellow
  { key: 'applied',    status: 'Applied',    label: 'Applied',    color: '#8b5cf6' }, // purple
  { key: 'client',     status: 'Client',     label: 'Client',     color: '#15803d' }, // green
  { key: 'rejected',   status: 'Rejected',   label: 'Rejected',   color: '#dc2626' }  // red
];

@Component({
  selector: 'app-pipeline',
  imports: [
    RouterLink, FormsModule,
    CdkDrag, CdkDropList, CdkDropListGroup, CdkDragPlaceholder,
    QuickActionModal
  ],
  templateUrl: './pipeline.html',
  styleUrl: './pipeline.scss'
})
export class Pipeline implements OnInit {
  private readonly contacts = inject(ContactService);
  private readonly activities = inject(ActivityService);
  private readonly messages = inject(MessageService);

  readonly columns = COLUMNS;
  readonly board = signal<Record<string, Contact[]>>({});
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly toast = signal<string | null>(null);

  /** All drop-list ids, so every column accepts drops from every other. */
  readonly listIds = COLUMNS.map(c => 'list-' + c.key);

  readonly total = computed(() =>
    Object.values(this.board()).reduce((n, list) => n + list.length, 0));

  // ---- quick add bar ----
  qaCompany = '';
  qaEmail = '';
  qaPhone = '';
  /** Which column the new contact lands in. */
  qaStatus: ContactStatus = 'Potential';
  readonly qaBusy = signal(false);

  // ---- bulk import ----
  readonly importOpen = signal(false);
  importText = '';
  readonly importBusy = signal(false);
  /** Which column the whole pasted batch lands in. */
  importStatus: ContactStatus = 'Potential';
  readonly importErrors = signal<string[]>([]);

  // ---- inline note (one card at a time) ----
  readonly noteFor = signal<string | null>(null);
  noteText = '';
  readonly noteBusy = signal(false);

  // ---- message modal ----
  readonly openAction = signal<QuickActionKind | null>(null);
  readonly actionBusy = signal(false);
  readonly actionError = signal<string | null>(null);
  private actionContact: Contact | null = null;

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.contacts.groupedByStatus().subscribe({
      next: (b) => { this.board.set(this.sortBoard(b)); this.loading.set(false); },
      error: () => { this.error.set('Could not load the board. Is the API running?'); this.loading.set(false); }
    });
  }

  contactsIn(key: string): Contact[] {
    return this.board()[key] ?? [];
  }

  /** Board key for a status, e.g. 'Client' -> 'client'. */
  private keyFor(status: ContactStatus): string {
    return COLUMNS.find(col => col.status === status)?.key ?? 'potential';
  }

  /**
   * Order inside a column:
   *   1. overdue next actions (soonest-overdue first)
   *   2. due today
   *   3. future dates, ascending
   *   4. no next action at all — last, by name
   */
  private sortBoard(board: Record<string, Contact[]>): Record<string, Contact[]> {
    const sorted: Record<string, Contact[]> = {};
    for (const [key, list] of Object.entries(board)) {
      sorted[key] = [...list].sort((a, b) => this.compare(a, b));
    }
    return sorted;
  }

  private compare(a: Contact, b: Contact): number {
    const da = this.daysUntil(a);
    const db = this.daysUntil(b);

    // Contacts without a next action always sink to the bottom.
    if (da === null && db === null) return a.fullName.localeCompare(b.fullName);
    if (da === null) return 1;
    if (db === null) return -1;

    // Both dated: ascending by date puts overdue first, then today, then future.
    if (da !== db) return da - db;
    return a.fullName.localeCompare(b.fullName);
  }


  // ---------- CDK drag & drop ----------
  drop(event: CdkDragDrop<Contact[]>, target: Column): void {
    // Reorder within the same column — nothing to persist.
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      return;
    }

    const contact = event.previousContainer.data[event.previousIndex];
    const previousStatus = contact.status;

    // 1. Optimistic: move the card now.
    transferArrayItem(
      event.previousContainer.data,
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );
    contact.status = target.status;
    this.board.set({ ...this.board() });   // nudge the signal

    // 2. Persist.
    this.contacts.updateStatus(contact.id, target.status).subscribe({
      next: () => this.flash(`${contact.fullName} → ${target.label}`),
      // 3. Revert on failure.
      error: () => {
        transferArrayItem(
          event.container.data,
          event.previousContainer.data,
          event.currentIndex,
          event.previousIndex
        );
        contact.status = previousStatus;
        this.board.set({ ...this.board() });
        this.flash('Could not move the contact');
      }
    });
  }

  // ---------- quick add ----------
  get quickAddInvalid(): boolean {
    return !this.qaCompany.trim();
  }

  /**
   * Adds a contact straight into "Suggested". The company field doubles as the
   * contact name here — this bar is for capturing a lead in one line.
   */
  quickAdd(): void {
    const name = this.qaCompany.trim();
    if (!name || this.qaBusy()) return;

    // "Acme Corp" -> first "Acme", last "Corp". A single word keeps the whole
    // name as the first name, with a blank last name.
    const parts = name.split(/\s+/);
    const firstName = parts[0];
    const lastName = parts.length > 1 ? parts.slice(1).join(' ') : '';

    this.qaBusy.set(true);
    this.contacts.create({
      firstName,
      lastName,
      email: this.qaEmail.trim() || null,
      phone: this.qaPhone.trim() || null,
      status: this.qaStatus
    }).subscribe({
      next: (created) => {
        // Insert instantly, no refetch.
        const key = this.keyFor(created.status);
        const board = { ...this.board() };
        board[key] = [created, ...(board[key] ?? [])];
        this.board.set(this.sortBoard(board));

        this.qaCompany = ''; this.qaEmail = ''; this.qaPhone = '';
        this.qaBusy.set(false);
        this.flash('Contact added');
      },
      error: () => {
        this.qaBusy.set(false);
        this.flash('Could not add the contact — check the email and phone');
      }
    });
  }

  // ---------- bulk import ----------
  toggleImport(): void {
    this.importOpen.update(v => !v);
    this.importErrors.set([]);
  }

  /** Rows parsed from the textarea — drives the live "N contacts" preview. */
  get parsedRows(): BulkImportRow[] {
    return this.parsePaste(this.importText);
  }

  /**
   * One contact per line: Name | email | phone
   * Also accepts tab or comma separators, and tolerates missing fields.
   */
  private parsePaste(text: string): BulkImportRow[] {
    return text
      .split(NEWLINE_RE)
      .map(line => line.trim())
      .filter(line => line.length > 0)
      .map(line => {
        const parts = line.split(SEPARATOR_RE).map(p => p.trim());
        return {
          name: parts[0] ?? '',
          email: parts[1] || null,
          phone: parts[2] || null
        };
      })
      .filter(r => r.name.length > 0);
  }

  runImport(): void {
    const rows = this.parsedRows;
    if (rows.length === 0 || this.importBusy()) return;

    this.importBusy.set(true);
    this.importErrors.set([]);

    this.contacts.bulkImport(rows, this.importStatus).subscribe({
      next: (res) => {
        this.importBusy.set(false);
        this.importErrors.set(res.errors);

        if (res.created > 0) {
          // Show the new cards immediately, no refetch.
          const key = this.keyFor(this.importStatus);
          const board = { ...this.board() };
          board[key] = [...res.contacts, ...(board[key] ?? [])];
          this.board.set(this.sortBoard(board));
        }

        if (res.skipped === 0) {
          this.importText = '';
          this.importOpen.set(false);
          this.flash(`Imported ${res.created} contact${res.created === 1 ? '' : 's'}`);
        } else {
          this.flash(`Imported ${res.created}, skipped ${res.skipped}`);
        }
      },
      error: () => {
        this.importBusy.set(false);
        this.importErrors.set(['Could not reach the server.']);
      }
    });
  }

  // ---------- inline note ----------
  startNote(c: Contact): void {
    this.noteText = '';
    this.noteFor.set(c.id);
  }

  cancelNote(): void {
    this.noteFor.set(null);
    this.noteText = '';
  }

  saveNote(c: Contact): void {
    const text = this.noteText.trim();
    if (!text || this.noteBusy()) return;

    this.noteBusy.set(true);
    this.activities.log({
      type: ACTIVITY_TYPE.Note, subject: 'Note',
      description: text, contactId: c.id
    }).subscribe({
      next: () => {
        this.noteBusy.set(false);
        this.cancelNote();
        this.flash('Note added');
        this.load();          // refresh so the timeline / board stay in sync
      },
      error: () => { this.noteBusy.set(false); this.flash('Could not save the note'); }
    });
  }

  // ---------- email / whatsapp ----------
  openMessage(c: Contact, kind: 'email' | 'whatsapp'): void {
    this.actionContact = c;
    this.actionError.set(null);
    this.openAction.set(kind);
  }

  closeAction(): void {
    this.openAction.set(null);
    this.actionBusy.set(false);
    this.actionError.set(null);
    this.actionContact = null;
  }

  defaultRecipient(): string {
    const c = this.actionContact;
    if (!c) return '';
    return this.openAction() === 'email' ? (c.email ?? '') : (c.phone ?? '');
  }

  handleAction(p: QuickActionPayload): void {
    const c = this.actionContact;
    if (!c) return;

    this.actionBusy.set(true);
    this.actionError.set(null);

    const isEmail = p.kind === 'email';
    this.messages.send({
      contactId: c.id,
      channel: isEmail ? CHANNEL.Email : CHANNEL.WhatsApp,
      recipient: p.recipient,
      subject: isEmail ? (p.subject || '(no subject)') : undefined,
      body: p.body
    }).subscribe({
      next: (r) => {
        this.actionBusy.set(false);
        if (r.status === 'Failed') {
          this.actionError.set(r.failureReason ?? 'Message failed to send.');
        } else {
          this.closeAction();
          this.flash(isEmail ? 'Email sent' : 'WhatsApp sent');
          this.load();        // the message is now on the contact's timeline
        }
      },
      error: () => { this.actionBusy.set(false); this.actionError.set('Could not reach the server.'); }
    });
  }

  // ---------- next action helpers ----------
  isOverdue(c: Contact): boolean {
    const d = this.daysUntil(c);
    return d !== null && d < 0;
  }

  dueLabel(c: Contact): string {
    const d = this.daysUntil(c);
    if (d === null) return '';
    if (d < -1) return `${Math.abs(d)}d overdue`;
    if (d === -1) return 'Yesterday';
    if (d === 0) return 'Today';
    if (d === 1) return 'Tomorrow';
    return `In ${d}d`;
  }

  private daysUntil(c: Contact): number | null {
    if (!c.nextActionDate) return null;
    const due = new Date(c.nextActionDate); due.setHours(0, 0, 0, 0);
    const today = new Date(); today.setHours(0, 0, 0, 0);
    return Math.round((due.getTime() - today.getTime()) / 86400000);
  }

  /** First letter of the name, for the avatar circle. */
  initial(c: Contact): string {
    return (c.firstName.charAt(0) || c.lastName.charAt(0) || '?').toUpperCase();
  }

  /** Avatar tint follows the contact's status. */
  avatarColor(c: Contact): string {
    return COLUMNS.find(col => col.status === c.status)?.color ?? '#94a3b8';
  }

  private flash(msg: string): void {
    this.toast.set(msg);
    setTimeout(() => this.toast.set(null), 2200);
  }
}
