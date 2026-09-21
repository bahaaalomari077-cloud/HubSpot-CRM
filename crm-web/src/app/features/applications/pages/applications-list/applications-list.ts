import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { ApplicationService } from '../../services/application.service';
import { Application } from '../../../../core/models/entities.model';

const STAGES = [
  { value: '', label: 'All stages' },
  { value: 0, label: 'New' },
  { value: 1, label: 'Create papers' },
  { value: 2, label: 'Invoice' },
  { value: 3, label: 'In progress' },
  { value: 4, label: 'Final invoice' },
  { value: 5, label: 'Won' },
  { value: 6, label: 'Lost' }
];

@Component({
  selector: 'app-applications-list',
  imports: [FormsModule, CurrencyPipe],
  templateUrl: './applications-list.html',
  styleUrl: './applications-list.scss'
})
export class ApplicationsList implements OnInit {
  private readonly service = inject(ApplicationService);
  readonly stages = STAGES;

  readonly items = signal<Application[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly totalCount = signal(0);

  // filters
  stageFilter: number | '' = '';
  minAmount?: number;
  search = '';

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.service.list({
      stage: this.stageFilter === '' ? undefined : Number(this.stageFilter),
      minAmount: this.minAmount || undefined,
      search: this.search || undefined,
      sortBy: 'amount',
      sortDescending: true,
      pageSize: 50
    }).subscribe({
      next: (res) => { this.items.set(res.items); this.totalCount.set(res.totalCount); this.loading.set(false); },
      error: () => { this.error.set('Failed to load applications.'); this.loading.set(false); }
    });
  }

  semanticClass(a: Application): string {
    return a.stageSemantic.toLowerCase(); // process | success | failure
  }
}
