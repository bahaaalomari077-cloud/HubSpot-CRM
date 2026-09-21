import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Company } from '../../../core/models/entities.model';
import { PagedResult, QueryParams } from '../../../core/models/common.model';
import { environment } from '../../../../environments/environment';
import { CompanyDetails } from '../company.model';

export interface CreateCompany {
  name: string;
  industry?: string | null;
  website?: string | null;
  phone?: string | null;
  email?: string | null;
  addressCity?: string | null;
  addressCountry?: string | null;
  annualRevenue?: number | null;
  employeeCount?: number | null;
}

@Injectable({ providedIn: 'root' })
export class CompanyService extends ApiService<Company, CreateCompany> {
  protected readonly resource = 'companies';

  list(query: QueryParams = {}): Observable<PagedResult<Company>> {
    return this.getPaged(query);
  }

  /** Company + its contacts + status breakdown, in one call. */
  details(id: string): Observable<CompanyDetails> {
    return this.http.get<CompanyDetails>(
      `${environment.apiBaseUrl}/companies/${id}/details`);
  }
}
