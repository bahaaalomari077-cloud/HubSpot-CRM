import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CompanyService } from '../../services/company.service';
import { CompanyDetails, CompanyContact } from '../../company.model';
import { CONTACT_STATUSES } from '../../../../core/models/common.model';

@Component({
  selector: 'app-company-details',
  imports: [RouterLink],
  templateUrl: './company-details.html',
  styleUrl: './company-details.scss'
})
export class CompanyDetailsPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(CompanyService);

  readonly data = signal<CompanyDetails | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.error.set('No company id.'); this.loading.set(false); return; }

    this.service.details(id).subscribe({
      next: (d) => { this.data.set(d); this.loading.set(false); },
      error: () => { this.error.set('Company not found.'); this.loading.set(false); }
    });
  }

  statusColor(status: string): string {
    return CONTACT_STATUSES.find(s => s.value === status)?.color ?? '#64748b';
  }

  /** Days until a contact's next action; negative = overdue. */
  private daysUntil(c: CompanyContact): number | null {
    if (!c.nextActionDate) return null;
    const due = new Date(c.nextActionDate); due.setHours(0, 0, 0, 0);
    const today = new Date(); today.setHours(0, 0, 0, 0);
    return Math.round((due.getTime() - today.getTime()) / 86400000);
  }

  isOverdue(c: CompanyContact): boolean {
    const d = this.daysUntil(c);
    return d !== null && d < 0;
  }

  dueLabel(c: CompanyContact): string {
    const d = this.daysUntil(c);
    if (d === null) return '';
    if (d < -1) return `${Math.abs(d)}d overdue`;
    if (d === -1) return 'Yesterday';
    if (d === 0) return 'Today';
    if (d === 1) return 'Tomorrow';
    return `In ${d}d`;
  }
}
