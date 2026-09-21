import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { ApplicationService } from '../../services/application.service';
import { Application } from '../../../../core/models/entities.model';
import { ApplicationStage } from '../../../../core/models/common.model';
import { ApplicationForm, ApplicationFormValue } from '../../components/application-form/application-form';

const STAGES = [
  { value: '', label: 'All stages', name: '' },
  { value: 0, label: 'New',           name: 'New' },
  { value: 1, label: 'Create papers', name: 'CreatePapers' },
  { value: 2, label: 'Invoice',       name: 'Invoice' },
  { value: 3, label: 'In progress',   name: 'InProgress' },
  { value: 4, label: 'Final invoice', name: 'FinalInvoice' },
  { value: 5, label: 'Won',           name: 'Won' },
  { value: 6, label: 'Lost',          name: 'Lost' }
];

@Component({
  selector: 'app-applications-list',
  imports: [FormsModule, CurrencyPipe, ApplicationForm],
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

  // ---- create / edit ----
  readonly formMode = signal<Application | 'new' | null>(null);
  readonly formBusy = signal(false);
  readonly formError = signal<string | null>(null);

  /** Message shown when the backend rejects a stage transition. */
  readonly stageError = signal<string | null>(null);

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

  // ---- form ----
  openCreate(): void { this.formError.set(null); this.formMode.set('new'); }
  openEdit(a: Application): void { this.formError.set(null); this.formMode.set(a); }
  closeForm(): void { this.formMode.set(null); this.formBusy.set(false); this.formError.set(null); }

  saveForm(v: ApplicationFormValue): void {
    const mode = this.formMode();
    if (!mode) return;
    this.formBusy.set(true);

    const done = () => { this.closeForm(); this.load(); };
    const fail = () => { this.formBusy.set(false); this.formError.set('Could not save.'); };

    if (mode === 'new') {
      this.service.create(v).subscribe({ next: done, error: fail });
    } else {
      this.service.update(mode.id, v).subscribe({ next: done, error: fail });
    }
  }

  /** Move a deal one stage. The backend enforces no-skip / no-backward. */
  moveStage(a: Application, stage: ApplicationStage): void {
    if (a.stage === stage) return;
    this.stageError.set(null);

    this.service.moveStage(a.id, stage).subscribe({
      next: () => this.load(),
      error: (e) => {
        const detail = e?.error?.detail ?? 'That stage move is not allowed.';
        this.stageError.set(detail);
        this.load();   // revert the dropdown to the stored stage
        setTimeout(() => this.stageError.set(null), 5000);
      }
    });
  }

  remove(a: Application): void {
    if (!confirm(`Delete "${a.title}"?`)) return;
    this.service.delete(a.id).subscribe(() => this.load());
  }

  semanticClass(a: Application): string {
    return a.stageSemantic.toLowerCase(); // process | success | failure
  }
}
