import { Component, inject, input, output, signal, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Application, Contact } from '../../../../core/models/entities.model';
import { ContactService } from '../../../contacts/services/contact.service';

export interface ApplicationFormValue {
  title: string;
  amount: number;
  currencyCode: string;
  probability: number;
  contactId: string | null;
}

@Component({
  selector: 'app-application-form',
  imports: [FormsModule],
  templateUrl: './application-form.html',
  styleUrl: './application-form.scss'
})
export class ApplicationForm {
  private readonly contactsApi = inject(ContactService);

  readonly mode = input.required<Application | 'new' | null>();
  readonly busy = input<boolean>(false);
  readonly error = input<string | null>(null);

  readonly saved = output<ApplicationFormValue>();
  readonly closed = output<void>();

  readonly contacts = signal<Contact[]>([]);

  title = '';
  amount = 0;
  currencyCode = 'USD';
  probability = 50;
  contactId = '';

  constructor() {
    effect(() => {
      const m = this.mode();
      if (!m) return;

      if (m === 'new') {
        this.title = ''; this.amount = 0; this.currencyCode = 'USD';
        this.probability = 50; this.contactId = '';
      } else {
        this.title = m.title; this.amount = m.amount;
        this.currencyCode = m.currencyCode; this.probability = m.probability;
        this.contactId = m.contactId ?? '';
      }

      this.contactsApi.list({ pageSize: 100, sortBy: 'lastName' }).subscribe({
        next: (r) => this.contacts.set(r.items),
        error: () => this.contacts.set([])
      });
    });
  }

  get isEdit(): boolean {
    const m = this.mode();
    return m !== null && m !== 'new';
  }

  get invalid(): boolean {
    return !this.title.trim() || this.amount < 0;
  }

  submit(): void {
    if (this.invalid || this.busy()) return;
    this.saved.emit({
      title: this.title.trim(),
      amount: Number(this.amount) || 0,
      currencyCode: this.currencyCode || 'USD',
      probability: Number(this.probability) || 0,
      contactId: this.contactId || null
    });
  }

  close(): void { this.closed.emit(); }
}
