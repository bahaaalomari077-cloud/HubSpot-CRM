import { Component, inject, input, output, signal, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Contact } from '../../../../core/models/entities.model';
import { Company } from '../../../../core/models/entities.model';
import { CONTACT_STATUSES, ContactStatus } from '../../../../core/models/common.model';
import { CompanyService } from '../../../companies/services/company.service';

/** Values the form emits; the parent decides create vs update. */
export interface ContactFormValue {
  firstName: string;
  lastName: string;
  email: string | null;
  phone: string | null;
  position: string | null;
  status: ContactStatus;
  companyId: string | null;
}

@Component({
  selector: 'app-contact-form',
  imports: [FormsModule],
  templateUrl: './contact-form.html',
  styleUrl: './contact-form.scss'
})
export class ContactForm {
  private readonly companiesApi = inject(CompanyService);

  /** null = closed. Pass a Contact to edit, or 'new' to create. */
  readonly mode = input.required<Contact | 'new' | null>();
  readonly busy = input<boolean>(false);
  readonly error = input<string | null>(null);

  readonly saved = output<ContactFormValue>();
  readonly closed = output<void>();

  readonly statuses = CONTACT_STATUSES;
  readonly companies = signal<Company[]>([]);

  firstName = '';
  lastName = '';
  email = '';
  phone = '';
  position = '';
  status: ContactStatus = 'Potential';
  companyId = '';

  constructor() {
    // Reset the fields whenever the dialog opens.
    effect(() => {
      const m = this.mode();
      if (!m) return;

      if (m === 'new') {
        this.firstName = ''; this.lastName = ''; this.email = '';
        this.phone = ''; this.position = ''; this.status = 'Potential'; this.companyId = '';
      } else {
        this.firstName = m.firstName; this.lastName = m.lastName;
        this.email = m.email ?? ''; this.phone = m.phone ?? '';
        this.position = m.position ?? ''; this.status = m.status;
        this.companyId = m.companyId ?? '';
      }

      // Load companies for the dropdown (once per open; cheap list).
      this.companiesApi.list({ pageSize: 100, sortBy: 'name' }).subscribe({
        next: (r) => this.companies.set(r.items),
        error: () => this.companies.set([])
      });
    });
  }

  get isEdit(): boolean {
    const m = this.mode();
    return m !== null && m !== 'new';
  }

  get invalid(): boolean {
    return !this.firstName.trim() || !this.lastName.trim();
  }

  submit(): void {
    if (this.invalid || this.busy()) return;
    this.saved.emit({
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      email: this.email.trim() || null,
      phone: this.phone.trim() || null,
      position: this.position.trim() || null,
      status: this.status,
      companyId: this.companyId || null
    });
  }

  close(): void { this.closed.emit(); }
}
