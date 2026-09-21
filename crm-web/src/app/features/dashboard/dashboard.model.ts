import { ContactStatus } from '../../core/models/common.model';

export interface StatusGroup {
  status: ContactStatus;
  count: number;
}

export interface UpcomingAction {
  contactId: string;
  contactName: string;
  status: ContactStatus;
  nextAction: string;
  nextActionDate?: string | null;
  isOverdue: boolean;
  daysUntil?: number | null;
}

export interface RecentItem {
  id: string;
  kind: 'Activity' | 'Message';
  subType: string;
  title: string;
  contactId?: string | null;
  contactName?: string | null;
  occurredAt: string;
}

export interface DashboardData {
  contactsByStatus: StatusGroup[];
  upcomingActions: UpcomingAction[];
  recentItems: RecentItem[];
  totalContacts: number;
  overdueCount: number;
}
