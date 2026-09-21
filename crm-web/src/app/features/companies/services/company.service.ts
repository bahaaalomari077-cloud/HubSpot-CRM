import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Company } from '../../../core/models/entities.model';

@Injectable({ providedIn: 'root' })
export class CompanyService extends ApiService<Company> {
  protected readonly resource = 'companies';
}
