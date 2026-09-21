/** Mirrors the backend PagedResult<T>. */
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

/** Base paging/search query params shared by every list endpoint. */
export interface QueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export type ApplicationStage =
  | 'New' | 'CreatePapers' | 'Invoice' | 'InProgress'
  | 'FinalInvoice' | 'Won' | 'Lost';

export type StageSemantic = 'Process' | 'Success' | 'Failure';

export type ActivityType = 'Meeting' | 'Call' | 'Task' | 'Email' | 'General' | 'UserAction';
export type ActivityStatus = 'Pending' | 'Completed' | 'CompletedAutomatically';

export type MessageChannel = 'Email' | 'Sms' | 'WhatsApp' | 'Telegram' | 'LiveChat' | 'Call';
export type MessageDirection = 'Inbound' | 'Outbound';
