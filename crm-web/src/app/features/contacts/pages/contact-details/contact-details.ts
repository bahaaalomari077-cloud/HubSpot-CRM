import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ContactService } from '../../services/contact.service';
import { TimelineService } from '../../../activities/services/timeline.service';
import { ActivityService, ACTIVITY_TYPE } from '../../../activities/services/activity.service';
import { MessageService, CHANNEL } from '../../../messages/services/message.service';
import { Timeline } from '../../../activities/components/timeline';
import {
  QuickActionModal, QuickActionKind, QuickActionPayload
} from '../../../../shared/components/quick-action/quick-action-modal';
import { Contact } from '../../../../core/models/entities.model';
import { TimelineGroup } from '../../../activities/timeline.model';
import { CONTACT_STATUSES, ContactStatus } from '../../../../core/models/common.model';

@Component({
  selector: 'app-contact-details',
  imports: [RouterLink, FormsModule, Timeline, QuickActionModal],
  templateUrl: './contact-details.html',
  styleUrl: './contact-details.scss'
})
export class ContactDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly contacts = inject(ContactService);
  private readonly timeline = inject(TimelineService);
  private readonly activities = inject(ActivityService);
  private readonly messages = inject(MessageService);

  readonly statuses = CONTACT_STATUSES;

  readonly contact = signal<Contact | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly groups = signal<TimelineGroup[]>([]);

  /** Small transient banner after an action succeeds. */
  readonly toast = signal<string | null>(null);

  // ---- quick action modal state ----
  readonly openAction = signal<QuickActionKind | null>(null);
  readonly actionBusy = signal(false);
  readonly actionError = signal<string | null>(null);

  // ---- inline quick-note ----
  noteText = '';
  readonly noteBusy = signal(false);

  // ---- next action editor ----
  nextActionText = '';
  nextActionDate = '';
  readonly editingNextAction = signal(false);
  readonly nextActionBusy = signal(false);

  private contactId = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.error.set('No contact id.'); this.loading.set(false); return; }
    this.contactId = id;

    this.contacts.getById(id).subscribe({
      next: (c) => { this.contact.set(c); this.loading.set(false); },
      error: () => { this.error.set('Contact not found.'); this.loading.set(false); }
    });

    this.loadTimeline();
  }

  private loadTimeline(): void {
    this.timeline.forContact(this.contactId).subscribe({
      next: (g) => this.groups.set(g),
      error: () => { /* timeline degrades gracefully */ }
    });
  }

  /** Status dropdown — persists immediately. */
  onStatusChange(status: ContactStatus): void {
    const current = this.contact();
    if (!current || current.status === status) return;

    // optimistic update, reverted on error
    const previous = current.status;
    this.contact.set({ ...current, status });

    this.contacts.updateStatus(this.contactId, status).subscribe({
      next: (updated) => { this.contact.set(updated); this.flash(`Status → ${status}`); },
      error: () => {
        this.contact.set({ ...current, status: previous });
        this.flash('Could not update status');
      }
    });
  }

  statusColor(status: string): string {
    return this.statuses.find(s => s.value === status)?.color ?? '#64748b';
  }

  // ---- quick actions ----
  open(kind: QuickActionKind): void {
    this.actionError.set(null);
    this.openAction.set(kind);
  }

  closeAction(): void {
    this.openAction.set(null);
    this.actionBusy.set(false);
    this.actionError.set(null);
  }

  /** Default recipient for the email / whatsapp modals. */
  defaultRecipient(): string {
    const c = this.contact();
    if (!c) return '';
    return this.openAction() === 'email' ? (c.email ?? '') : (c.phone ?? '');
  }

  /** Inline note: Enter (or the button) saves, then the timeline refreshes. */
  saveNote(): void {
    const text = this.noteText.trim();
    if (!text || this.noteBusy()) return;

    this.noteBusy.set(true);
    this.activities.log({
      type: ACTIVITY_TYPE.Note, subject: 'Note',
      description: text, contactId: this.contactId
    }).subscribe({
      next: () => {
        this.noteText = '';
        this.noteBusy.set(false);
        this.loadTimeline();
        this.flash('Note added');
      },
      error: () => { this.noteBusy.set(false); this.flash('Could not save the note'); }
    });
  }

  // ---- next action ----
  startEditNextAction(): void {
    const c = this.contact();
    this.nextActionText = c?.nextAction ?? '';
    this.nextActionDate = c?.nextActionDate ? c.nextActionDate.substring(0, 10) : '';
    this.editingNextAction.set(true);
  }

  cancelNextAction(): void {
    this.editingNextAction.set(false);
  }

  saveNextAction(): void {
    const text = this.nextActionText.trim();
    this.nextActionBusy.set(true);

    const date = text && this.nextActionDate
      ? new Date(this.nextActionDate + 'T09:00:00').toISOString()
      : null;

    this.contacts.updateNextAction(this.contactId, text || null, date).subscribe({
      next: (updated) => {
        this.contact.set(updated);
        this.nextActionBusy.set(false);
        this.editingNextAction.set(false);
        this.flash(text ? 'Next action saved' : 'Next action cleared');
      },
      error: () => { this.nextActionBusy.set(false); this.flash('Could not save next action'); }
    });
  }

  /** Days until the next action; negative means overdue. */
  nextActionDays(): number | null {
    const iso = this.contact()?.nextActionDate;
    if (!iso) return null;
    const due = new Date(iso); due.setHours(0, 0, 0, 0);
    const today = new Date(); today.setHours(0, 0, 0, 0);
    return Math.round((due.getTime() - today.getTime()) / 86400000);
  }

  nextActionLabel(): string {
    const d = this.nextActionDays();
    if (d === null) return '';
    if (d < -1) return `${Math.abs(d)} days overdue`;
    if (d === -1) return 'Yesterday';
    if (d === 0) return 'Today';
    if (d === 1) return 'Tomorrow';
    return `In ${d} days`;
  }

  handleAction(p: QuickActionPayload): void {
    this.actionBusy.set(true);
    this.actionError.set(null);

    const done = (msg: string) => {
      this.actionBusy.set(false);
      this.closeAction();
      this.loadTimeline();     // refresh so the new entry appears
      this.flash(msg);
    };
    const fail = (msg: string) => {
      this.actionBusy.set(false);
      this.actionError.set(msg);
    };

    switch (p.kind) {
      case 'note':
        this.activities.log({
          type: ACTIVITY_TYPE.Note, subject: 'Note',
          description: p.body, contactId: this.contactId
        }).subscribe({ next: () => done('Note added'), error: () => fail('Could not save the note.') });
        break;

      case 'call':
        this.activities.log({
          type: ACTIVITY_TYPE.Call, subject: p.subject || 'Call',
          description: p.body, contactId: this.contactId
        }).subscribe({ next: () => done('Call logged'), error: () => fail('Could not log the call.') });
        break;

      case 'email':
        this.messages.send({
          contactId: this.contactId, channel: CHANNEL.Email,
          recipient: p.recipient, subject: p.subject || '(no subject)', body: p.body
        }).subscribe({
          next: (r) => r.status === 'Failed'
            ? fail(r.failureReason ?? 'Email failed to send.')
            : done('Email sent'),
          error: () => fail('Could not reach the server.')
        });
        break;

      case 'whatsapp':
        this.messages.send({
          contactId: this.contactId, channel: CHANNEL.WhatsApp,
          recipient: p.recipient, body: p.body
        }).subscribe({
          next: (r) => r.status === 'Failed'
            ? fail(r.failureReason ?? 'WhatsApp failed to send.')
            : done('WhatsApp sent'),
          error: () => fail('Could not reach the server.')
        });
        break;
    }
  }

  private flash(msg: string): void {
    this.toast.set(msg);
    setTimeout(() => this.toast.set(null), 2500);
  }
}
