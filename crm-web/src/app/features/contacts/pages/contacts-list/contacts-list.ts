import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ContactService } from '../../services/contact.service';
import { Contact } from '../../../../core/models/entities.model';
import { CONTACT_STATUSES } from '../../../../core/models/common.model';
import { ContactForm, ContactFormValue } from '../../components/contact-form/contact-form';

@Component({
  selector: 'app-contacts-list',
  imports: [RouterLink, FormsModule, ContactForm],
  templateUrl: './contacts-list.html',
  styleUrl: './contacts-list.scss'
})
export class ContactsList implements OnInit {
  private readonly service = inject(ContactService);
  private readonly route = inject(ActivatedRoute);

  /** Set when arriving from a company's contact-count link. */
  companyFilter: string | null = null;

  readonly statuses = CONTACT_STATUSES;
  statusFilter: number | '' = '';

  // ---- create / edit form ----
  readonly formMode = signal<Contact | 'new' | null>(null);
  readonly formBusy = signal(false);
  readonly formError = signal<string | null>(null);

  // ---- reactive state via signals ----
  readonly contacts = signal<Contact[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // paging + filtering
  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);
  search = '';

  ngOnInit(): void {
    // Deep links: /contacts?status=2  or  /contacts?companyId=...
    const qp = this.route.snapshot.queryParamMap;
    const status = qp.get('status');
    if (status !== null && status !== '') this.statusFilter = Number(status);
    this.companyFilter = qp.get('companyId');

    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.service.list({
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.search || undefined,
      status: this.statusFilter === '' ? undefined : Number(this.statusFilter),
      companyId: this.companyFilter ?? undefined,
      sortBy: 'lastName'
    }).subscribe({
      next: (res) => {
        this.contacts.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load contacts. Is the API running?');
        this.loading.set(false);
      }
    });
  }

  clearCompanyFilter(): void {
    this.companyFilter = null;
    this.page.set(1);
    this.load();
  }

  onSearch(): void {
    this.page.set(1);
    this.load();
  }

  prev(): void {
    if (this.page() > 1) { this.page.update(p => p - 1); this.load(); }
  }

  next(): void {
    if (this.page() < this.totalPages()) { this.page.update(p => p + 1); this.load(); }
  }

  statusColor(status: string): string {
    return this.statuses.find(s => s.value === status)?.color ?? '#64748b';
  }

  /** Days until a contact's next action; negative = overdue. */
  private daysUntil(c: Contact): number | null {
    if (!c.nextActionDate) return null;
    const due = new Date(c.nextActionDate); due.setHours(0, 0, 0, 0);
    const today = new Date(); today.setHours(0, 0, 0, 0);
    return Math.round((due.getTime() - today.getTime()) / 86400000);
  }

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

  // ---- form handlers ----
  openCreate(): void { this.formError.set(null); this.formMode.set('new'); }
  openEdit(c: Contact): void { this.formError.set(null); this.formMode.set(c); }
  closeForm(): void { this.formMode.set(null); this.formBusy.set(false); this.formError.set(null); }

  saveForm(v: ContactFormValue): void {
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

  remove(c: Contact): void {
    if (!confirm(`Delete ${c.fullName}?`)) return;
    this.service.delete(c.id).subscribe(() => this.load());
  }
}
