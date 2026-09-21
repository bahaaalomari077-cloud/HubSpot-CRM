import { Company } from '../../core/models/entities.model';
import { ContactStatus } from '../../core/models/common.model';

export interface CompanyStatusCount {
  status: ContactStatus;
  count: number;
}

export interface CompanyContact {
  id: string;
  fullName: string;
  email?: string | null;
  phone?: string | null;
  position?: string | null;
  status: ContactStatus;
  nextAction?: string | null;
  nextActionDate?: string | null;
}

export interface CompanyDetails {
  company: Company;
  contacts: CompanyContact[];
  contactsByStatus: CompanyStatusCount[];
  activeContacts: number;
  isActive: boolean;
}
