import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Activity } from '../../../core/models/entities.model';
import { PagedResult, QueryParams } from '../../../core/models/common.model';

/** Activity type ids as the backend enum expects them. */
export const ACTIVITY_TYPE = { Meeting: 1, Call: 2, Task: 3, Email: 4, Note: 7 } as const;

export interface ActivityQuery extends QueryParams {
  contactId?: string;
  applicationId?: string;
  assignedToId?: string;
}

export interface CreateActivity {
  type: number;           // ACTIVITY_TYPE.Note | .Call | .Meeting
  subject?: string;
  description?: string;
  contactId: string;
  applicationId?: string | null;
}

@Injectable({ providedIn: 'root' })
export class ActivityService extends ApiService<Activity, CreateActivity> {
  protected readonly resource = 'activities';

  list(query: ActivityQuery = {}): Observable<PagedResult<Activity>> {
    return this.getPaged(query);
  }

  /** Log a note / call / meeting against a contact. */
  log(dto: CreateActivity): Observable<Activity> {
    return this.create(dto);
  }
}
