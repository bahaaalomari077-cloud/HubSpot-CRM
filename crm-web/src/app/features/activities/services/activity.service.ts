import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Activity } from '../../../core/models/entities.model';
import { PagedResult, QueryParams } from '../../../core/models/common.model';

export interface ActivityQuery extends QueryParams {
  contactId?: string;
  applicationId?: string;
  assignedToId?: string;
}

@Injectable({ providedIn: 'root' })
export class ActivityService extends ApiService<Activity> {
  protected readonly resource = 'activities';

  /** Timeline for one contact — newest first. */
  forContact(contactId: string): Observable<PagedResult<Activity>> {
    return this.getPaged({ contactId, sortBy: 'createdAt', sortDescending: true, pageSize: 100 });
  }

  list(query: ActivityQuery = {}): Observable<PagedResult<Activity>> {
    return this.getPaged(query);
  }
}
