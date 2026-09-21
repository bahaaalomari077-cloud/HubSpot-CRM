import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { CompanyService } from '../../services/company.service';
import { Company } from '../../../../core/models/entities.model';
import { CompanyForm, CompanyFormValue } from '../../components/company-form/company-form';

@Component({
  selector: 'app-companies-list',
  imports: [RouterLink, FormsModule, CompanyForm],
  templateUrl: './companies-list.html',
  styleUrl: './companies-list.scss'
})
export class CompaniesList implements OnInit {
  private readonly service = inject(CompanyService);

  readonly companies = signal<Company[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);
  search = '';

  // ---- create / edit ----
  readonly formMode = signal<Company | 'new' | null>(null);
  readonly formBusy = signal(false);
  readonly formError = signal<string | null>(null);

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.service.list({
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.search || undefined,
      sortBy: 'name'
    }).subscribe({
      next: (res) => {
        this.companies.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load companies. Is the API running?');
        this.loading.set(false);
      }
    });
  }

  onSearch(): void { this.page.set(1); this.load(); }

  prev(): void { if (this.page() > 1) { this.page.update(p => p - 1); this.load(); } }
  next(): void { if (this.page() < this.totalPages()) { this.page.update(p => p + 1); this.load(); } }

  // ---- form ----
  openCreate(): void { this.formError.set(null); this.formMode.set('new'); }
  openEdit(c: Company): void { this.formError.set(null); this.formMode.set(c); }
  closeForm(): void { this.formMode.set(null); this.formBusy.set(false); this.formError.set(null); }

  saveForm(v: CompanyFormValue): void {
    const mode = this.formMode();
    if (!mode) return;

    this.formBusy.set(true);
    this.formError.set(null);

    const done = () => { this.closeForm(); this.load(); };
    const fail = () => {
      this.formBusy.set(false);
      this.formError.set('Could not save. Check the email and phone format.');
    };

    if (mode === 'new') {
      this.service.create(v).subscribe({ next: done, error: fail });
    } else {
      this.service.update(mode.id, v).subscribe({ next: done, error: fail });
    }
  }

  remove(c: Company): void {
    const n = c.contactCount ?? 0;
    const warning = n > 0
      ? `Delete "${c.name}"? Its ${n} contact${n === 1 ? '' : 's'} will be kept but unlinked.`
      : `Delete "${c.name}"?`;
    if (!confirm(warning)) return;
    this.service.delete(c.id).subscribe(() => this.load());
  }
}
