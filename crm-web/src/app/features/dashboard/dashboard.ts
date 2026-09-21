import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ContactService } from '../contacts/services/contact.service';
import { CompanyService } from '../companies/services/company.service';
import { ApplicationService } from '../applications/services/application.service';
import { PagedResult } from '../../core/models/common.model';

/** Empty page used when an endpoint isn't available yet (e.g. no CompaniesController). */
const EMPTY_PAGE: PagedResult<any> = {
  items: [], page: 1, pageSize: 0, totalCount: 0, totalPages: 0, hasPrevious: false, hasNext: false
};

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, CurrencyPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private readonly contacts = inject(ContactService);
  private readonly companies = inject(CompanyService);
  private readonly applications = inject(ApplicationService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly contactCount = signal(0);
  readonly companyCount = signal(0);
  readonly openCount = signal(0);
  readonly pipelineValue = signal(0);
  readonly wonCount = signal(0);

  ngOnInit(): void {
    // A single page-size=1 call per resource gives us totalCount cheaply.
    forkJoin({
      contacts: this.contacts.list({ pageSize: 1 }).pipe(catchError(() => of(EMPTY_PAGE))),
      companies: this.companies.getPaged({ pageSize: 1 }).pipe(catchError(() => of(EMPTY_PAGE))),
      open: this.applications.list({ isClosed: false, pageSize: 100 }).pipe(catchError(() => of(EMPTY_PAGE))),
      won: this.applications.list({ stage: 5, pageSize: 1 }).pipe(catchError(() => of(EMPTY_PAGE)))
    }).subscribe({
      next: ({ contacts, companies, open, won }) => {
        this.contactCount.set(contacts.totalCount);
        this.companyCount.set(companies.totalCount);
        this.openCount.set(open.totalCount);
        this.pipelineValue.set(open.items.reduce((sum, a) => sum + a.amount, 0));
        this.wonCount.set(won.totalCount);
        this.loading.set(false);
      },
      error: () => { this.error.set('Failed to load dashboard. Is the API running?'); this.loading.set(false); }
    });
  }
}
