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

/** Where a contact stands. Set manually by the admin — no workflow rules. */
export type ContactStatus =
  | 'Potential' | 'Contacted' | 'Interested'
  | 'Applied' | 'Client' | 'Rejected';

/** Ordered list for dropdowns, with display labels and a colour per status. */
export const CONTACT_STATUSES: { value: ContactStatus; label: string; color: string }[] = [
  { value: 'Potential',  label: 'Potential',  color: '#64748b' },  // gray
  { value: 'Contacted',  label: 'Contacted',  color: '#2f6df6' },  // blue
  { value: 'Interested', label: 'Interested', color: '#d97706' },  // yellow/amber
  { value: 'Applied',    label: 'Applied',    color: '#8b5cf6' },  // purple
  { value: 'Client',     label: 'Client',     color: '#15803d' },  // green
  { value: 'Rejected',   label: 'Rejected',   color: '#dc2626' }   // red
];

export type ApplicationStage =
  | 'New' | 'CreatePapers' | 'Invoice' | 'InProgress'
  | 'FinalInvoice' | 'Won' | 'Lost';

export type StageSemantic = 'Process' | 'Success' | 'Failure';

export type ActivityType = 'Meeting' | 'Call' | 'Task' | 'Email' | 'General' | 'UserAction';
export type ActivityStatus = 'Pending' | 'Completed' | 'CompletedAutomatically';

export type MessageChannel = 'Email' | 'Sms' | 'WhatsApp' | 'Telegram' | 'LiveChat' | 'Call';
export type MessageDirection = 'Inbound' | 'Outbound';
