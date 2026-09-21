import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Message } from '../../../core/models/entities.model';
import { PagedResult } from '../../../core/models/common.model';

@Injectable({ providedIn: 'root' })
export class MessageService extends ApiService<Message> {
  protected readonly resource = 'messages';

  forContact(contactId: string): Observable<PagedResult<Message>> {
    return this.getPaged({ contactId, sortBy: 'sentAt', sortDescending: true, pageSize: 100 });
  }
}
