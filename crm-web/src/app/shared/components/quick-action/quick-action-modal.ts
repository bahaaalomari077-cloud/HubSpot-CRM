import { Component, computed, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

export type QuickActionKind = 'note' | 'call' | 'email' | 'whatsapp';

/** What the modal emits when the admin submits. */
export interface QuickActionPayload {
  kind: QuickActionKind;
  subject: string;
  body: string;
  recipient: string;
}

const CONFIG: Record<QuickActionKind, {
  title: string; bodyLabel: string; needsSubject: boolean;
  needsRecipient: boolean; recipientLabel: string; cta: string;
}> = {
  note:     { title: 'Add Note',      bodyLabel: 'Note',    needsSubject: false, needsRecipient: false, recipientLabel: '',       cta: 'Save note' },
  call:     { title: 'Log Call',      bodyLabel: 'Summary', needsSubject: true,  needsRecipient: false, recipientLabel: '',       cta: 'Log call' },
  email:    { title: 'Send Email',    bodyLabel: 'Message', needsSubject: true,  needsRecipient: true,  recipientLabel: 'To (email)', cta: 'Send email' },
  whatsapp: { title: 'Send WhatsApp', bodyLabel: 'Message', needsSubject: false, needsRecipient: true,  recipientLabel: 'To (phone)', cta: 'Send WhatsApp' }
};

@Component({
  selector: 'app-quick-action-modal',
  imports: [FormsModule],
  templateUrl: './quick-action-modal.html',
  styleUrl: './quick-action-modal.scss'
})
export class QuickActionModal {
  /** Which action is open; null closes the modal. */
  readonly kind = input.required<QuickActionKind | null>();
  /** Prefilled recipient (contact's email or phone). */
  readonly defaultRecipient = input<string>('');
  /** Set while the request is in flight. */
  readonly busy = input<boolean>(false);
  /** Error text from the last attempt, if any. */
  readonly error = input<string | null>(null);

  readonly submitted = output<QuickActionPayload>();
  readonly closed = output<void>();

  subject = '';
  body = '';
  recipient = '';

  readonly config = computed(() => {
    const k = this.kind();
    return k ? CONFIG[k] : null;
  });

  /** Reset fields whenever a new action opens. */
  private lastKind: QuickActionKind | null = null;
  readonly _sync = computed(() => {
    const k = this.kind();
    if (k !== this.lastKind) {
      this.lastKind = k;
      this.subject = '';
      this.body = '';
      this.recipient = this.defaultRecipient();
    }
    return k;
  });

  submit(): void {
    const k = this.kind();
    if (!k || !this.body.trim()) return;
    this.submitted.emit({
      kind: k,
      subject: this.subject.trim(),
      body: this.body.trim(),
      recipient: this.recipient.trim()
    });
  }

  close(): void {
    this.closed.emit();
  }

  /** Disable the submit button when required fields are empty. */
  get invalid(): boolean {
    const c = this.config();
    if (!c) return true;
    if (!this.body.trim()) return true;
    if (c.needsRecipient && !this.recipient.trim()) return true;
    return false;
  }
}
