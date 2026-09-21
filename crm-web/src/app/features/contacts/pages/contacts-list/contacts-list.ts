import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ContactService } from '../../services/contact.service';
import { Contact } from '../../../../core/models/entities.model';

@Component({
  selector: 'app-contacts-list',
  imports: [RouterLink, FormsModule],
  templateUrl: './contacts-list.html',
  styleUrl: './contacts-list.scss'
})
export class ContactsList implements OnInit {
  private readonly service = inject(ContactService);

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
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.service.list({
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.search || undefined,
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

  remove(c: Contact): void {
    if (!confirm(`Delete ${c.fullName}?`)) return;
    this.service.delete(c.id).subscribe(() => this.load());
  }
}
