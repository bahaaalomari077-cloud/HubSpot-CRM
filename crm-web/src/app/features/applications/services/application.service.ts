import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ApiService } from '../../../core/services/api.service';
import { Application, CreateApplication } from '../../../core/models/entities.model';
import { PagedResult, QueryParams, ApplicationStage } from '../../../core/models/common.model';
import { environment } from '../../../../environments/environment';

export interface ApplicationQuery extends QueryParams {
  stage?: number;
  contactId?: string;
  companyId?: string;
  assignedToId?: string;
  isClosed?: boolean;
  minAmount?: number;
  maxAmount?: number;
}

@Injectable({ providedIn: 'root' })
export class ApplicationService extends ApiService<Application, CreateApplication> {
  protected readonly resource = 'applications';

  list(query: ApplicationQuery = {}): Observable<PagedResult<Application>> {
    return this.getPaged(query);
  }

  /** Kanban stage move — hits the dedicated PATCH endpoint. */
  moveStage(id: string, stage: ApplicationStage): Observable<Application> {
    return this.http.patch<Application>(
      `${environment.apiBaseUrl}/applications/${id}/stage`, { stage });
  }
}
