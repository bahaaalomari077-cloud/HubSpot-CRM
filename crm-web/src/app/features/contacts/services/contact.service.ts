import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Contact, CreateContact } from '../../../core/models/entities.model';
import { PagedResult, QueryParams, ContactStatus } from '../../../core/models/common.model';
import { environment } from '../../../../environments/environment';

export interface BulkImportRow {
  name: string;
  email?: string | null;
  phone?: string | null;
}

export interface BulkImportResult {
  created: number;
  skipped: number;
  errors: string[];
  contacts: Contact[];
}

export interface ContactQuery extends QueryParams {
  companyId?: string;
  assignedToId?: string;
  /** 0 potential · 1 contacted · 2 interested · 3 applied · 4 client · 5 rejected */
  status?: number;
}

@Injectable({ providedIn: 'root' })
export class ContactService extends ApiService<Contact, CreateContact> {
  protected readonly resource = 'contacts';

  list(query: ContactQuery = {}): Observable<PagedResult<Contact>> {
    return this.getPaged(query);
  }

  /** Import a pasted batch. Bad rows are skipped server-side with reasons. */
  bulkImport(rows: BulkImportRow[], status: ContactStatus = 'Potential'): Observable<BulkImportResult> {
    return this.http.post<BulkImportResult>(
      `${environment.apiBaseUrl}/contacts/bulk-import`, { rows, status });
  }

  /** All contacts grouped by status, for the kanban board. */
  groupedByStatus(): Observable<Record<string, Contact[]>> {
    return this.http.get<Record<string, Contact[]>>(
      `${environment.apiBaseUrl}/contacts?groupBy=status`);
  }

  /** Manual status change — instant, no workflow rules. */
  updateStatus(id: string, status: ContactStatus): Observable<Contact> {
    return this.http.patch<Contact>(
      `${environment.apiBaseUrl}/contacts/${id}/status`, { status });
  }

  /** Set or clear what to do next with this contact. */
  updateNextAction(id: string, nextAction: string | null, nextActionDate: string | null): Observable<Contact> {
    return this.http.patch<Contact>(
      `${environment.apiBaseUrl}/contacts/${id}/next-action`, { nextAction, nextActionDate });
  }
}
