import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardData } from './dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  /** One call returns status groups, upcoming actions and recent items. */
  get(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${environment.apiBaseUrl}/dashboard`);
  }
}
