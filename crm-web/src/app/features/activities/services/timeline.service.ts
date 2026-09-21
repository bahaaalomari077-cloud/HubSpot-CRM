import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TimelineGroup } from '../timeline.model';

@Injectable({ providedIn: 'root' })
export class TimelineService {
  private readonly http = inject(HttpClient);

  /** One call — backend merges activities + messages, sorted and grouped. */
  forContact(contactId: string): Observable<TimelineGroup[]> {
    return this.http.get<TimelineGroup[]>(
      `${environment.apiBaseUrl}/contacts/${contactId}/timeline`);
  }
}
