import { Component, effect, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Company } from '../../../../core/models/entities.model';

export interface CompanyFormValue {
  name: string;
  industry: string | null;
  website: string | null;
  phone: string | null;
  email: string | null;
  addressCity: string | null;
  addressCountry: string | null;
  employeeCount: number | null;
  annualRevenue: number | null;
}

@Component({
  selector: 'app-company-form',
  imports: [FormsModule],
  templateUrl: './company-form.html',
  styleUrl: './company-form.scss'
})
export class CompanyForm {
  /** null = closed. Pass a Company to edit, or 'new' to create. */
  readonly mode = input.required<Company | 'new' | null>();
  readonly busy = input<boolean>(false);
  readonly error = input<string | null>(null);

  readonly saved = output<CompanyFormValue>();
  readonly closed = output<void>();

  name = '';
  industry = '';
  website = '';
  phone = '';
  email = '';
  addressCity = '';
  addressCountry = '';
  employeeCount: number | null = null;
  annualRevenue: number | null = null;

  constructor() {
    effect(() => {
      const m = this.mode();
      if (!m) return;

      if (m === 'new') {
        this.name = ''; this.industry = ''; this.website = '';
        this.phone = ''; this.email = '';
        this.addressCity = ''; this.addressCountry = '';
        this.employeeCount = null; this.annualRevenue = null;
      } else {
        this.name = m.name;
        this.industry = m.industry ?? '';
        this.website = m.website ?? '';
        this.phone = m.phone ?? '';
        this.email = m.email ?? '';
        this.addressCity = m.addressCity ?? '';
        this.addressCountry = m.addressCountry ?? '';
        this.employeeCount = m.employeeCount ?? null;
        this.annualRevenue = m.annualRevenue ?? null;
      }
    });
  }

  get isEdit(): boolean {
    const m = this.mode();
    return m !== null && m !== 'new';
  }

  get invalid(): boolean {
    return !this.name.trim();
  }

  submit(): void {
    if (this.invalid || this.busy()) return;
    this.saved.emit({
      name: this.name.trim(),
      industry: this.industry.trim() || null,
      website: this.website.trim() || null,
      phone: this.phone.trim() || null,
      email: this.email.trim() || null,
      addressCity: this.addressCity.trim() || null,
      addressCountry: this.addressCountry.trim() || null,
      employeeCount: this.employeeCount !== null ? Number(this.employeeCount) : null,
      annualRevenue: this.annualRevenue !== null ? Number(this.annualRevenue) : null
    });
  }

  close(): void { this.closed.emit(); }
}
