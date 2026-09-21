import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { DashboardService } from './dashboard.service';
import { DashboardData, UpcomingAction } from './dashboard.model';
import { CONTACT_STATUSES } from '../../core/models/common.model';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private readonly service = inject(DashboardService);

  readonly data = signal<DashboardData | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.service.get().subscribe({
      next: (d) => { this.data.set(d); this.loading.set(false); },
      error: () => { this.error.set('Could not load the dashboard. Is the API running?'); this.loading.set(false); }
    });
  }

  statusColor(status: string): string {
    return CONTACT_STATUSES.find(s => s.value === status)?.color ?? '#64748b';
  }

  /** Status index, so a tile can deep-link into the filtered contacts list. */
  statusIndex(status: string): number {
    return CONTACT_STATUSES.findIndex(s => s.value === status);
  }

  /** Human phrasing for when an action is due. */
  dueLabel(a: UpcomingAction): string {
    if (a.daysUntil === null || a.daysUntil === undefined) return 'No date';
    if (a.daysUntil < -1) return `${Math.abs(a.daysUntil)} days overdue`;
    if (a.daysUntil === -1) return 'Yesterday';
    if (a.daysUntil === 0) return 'Today';
    if (a.daysUntil === 1) return 'Tomorrow';
    return `In ${a.daysUntil} days`;
  }

  /** Icon for a recent timeline item. */
  icon(subType: string): string {
    switch (subType) {
      case 'Call': return '📞';
      case 'Meeting': return '👥';
      case 'Note': return '📝';
      case 'Email': return '✉️';
      case 'WhatsApp': return '💬';
      case 'Sms': return '💬';
      default: return '•';
    }
  }
}
