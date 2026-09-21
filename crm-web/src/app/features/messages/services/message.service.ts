import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Message } from '../../../core/models/entities.model';
import { PagedResult } from '../../../core/models/common.model';
import { environment } from '../../../../environments/environment';

/** Channel ids as the backend enum expects them. */
export const CHANNEL = { Email: 0, Sms: 1, WhatsApp: 2 } as const;

export interface SendMessageRequest {
  contactId: string;
  channel: number;        // CHANNEL.Email | CHANNEL.WhatsApp
  recipient: string;      // email address or phone
  subject?: string;
  body: string;
}

export interface SendMessageResult {
  messageId: string;
  status: string;         // Sent | Failed | Queued …
  providerMessageId?: string | null;
  failureReason?: string | null;
}

@Injectable({ providedIn: 'root' })
export class MessageService extends ApiService<Message> {
  protected readonly resource = 'messages';

  forContact(contactId: string): Observable<PagedResult<Message>> {
    return this.getPaged({ contactId, sortBy: 'sentAt', sortDescending: true, pageSize: 100 });
  }

  /** Send an email or WhatsApp message. Stored and status-tracked by the backend. */
  send(req: SendMessageRequest): Observable<SendMessageResult> {
    return this.http.post<SendMessageResult>(
      `${environment.apiBaseUrl}/messages/send`, req);
  }
}
