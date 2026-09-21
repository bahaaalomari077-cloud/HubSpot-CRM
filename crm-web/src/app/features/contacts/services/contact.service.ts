import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Contact, CreateContact } from '../../../core/models/entities.model';
import { PagedResult, QueryParams } from '../../../core/models/common.model';

export interface ContactQuery extends QueryParams {
  companyId?: string;
  assignedToId?: string;
}

@Injectable({ providedIn: 'root' })
export class ContactService extends ApiService<Contact, CreateContact> {
  protected readonly resource = 'contacts';

  list(query: ContactQuery = {}): Observable<PagedResult<Contact>> {
    return this.getPaged(query);
  }
}
