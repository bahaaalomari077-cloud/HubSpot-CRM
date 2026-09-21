import {
  ApplicationStage, StageSemantic, ContactStatus,
  ActivityType, ActivityStatus,
  MessageChannel, MessageDirection
} from './common.model';

export interface Contact {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email?: string | null;
  phone?: string | null;
  position?: string | null;
  status: ContactStatus;
  nextAction?: string | null;
  nextActionDate?: string | null;
  companyId?: string | null;
  companyName?: string | null;
  assignedToId?: string | null;
  assignedToName?: string | null;
  createdAt: string;
}

export interface CreateContact {
  firstName: string;
  lastName: string;
  email?: string | null;
  phone?: string | null;
  position?: string | null;
  /** Defaults to Potential on the server when omitted. */
  status?: ContactStatus;
  companyId?: string | null;
  assignedToId?: string | null;
}

export interface Company {
  id: string;
  name: string;
  industry?: string | null;
  website?: string | null;
  phone?: string | null;
  email?: string | null;
  addressCity?: string | null;
  addressCountry?: string | null;
  annualRevenue?: number | null;
  employeeCount?: number | null;
  contactCount?: number;
  activeContacts?: number;
  assignedToId?: string | null;
  createdAt: string;
}

export interface Application {
  id: string;
  title: string;
  stage: ApplicationStage;
  stageSemantic: StageSemantic;
  isClosed: boolean;
  amount: number;
  currencyCode: string;
  probability: number;
  beginDate?: string | null;
  closeDate?: string | null;
  contactId?: string | null;
  contactName?: string | null;
  companyId?: string | null;
  companyName?: string | null;
  assignedToId?: string | null;
  assignedToName?: string | null;
  createdAt: string;
}

export interface CreateApplication {
  title: string;
  amount: number;
  currencyCode: string;
  probability: number;
  stage?: ApplicationStage;
  contactId?: string | null;
  companyId?: string | null;
  assignedToId?: string | null;
}

export interface Activity {
  id: string;
  subject: string;
  description?: string | null;
  type: ActivityType;
  status: ActivityStatus;
  startTime?: string | null;
  deadline?: string | null;
  isCompleted: boolean;
  contactId?: string | null;
  applicationId?: string | null;
  assignedToId?: string | null;
  createdAt: string;
}

export interface Message {
  id: string;
  channel: MessageChannel;
  direction: MessageDirection;
  subject?: string | null;
  body: string;
  sentAt: string;
  isRead: boolean;
  contactId: string;
  userId?: string | null;
}
