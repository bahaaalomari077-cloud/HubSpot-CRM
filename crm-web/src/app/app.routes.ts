import { Routes } from '@angular/router';
import { Shell } from './layout/shell';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      { path: '', redirectTo: 'pipeline', pathMatch: 'full' },
      {
        path: 'pipeline',
        loadComponent: () => import('./features/pipeline/pipeline').then(m => m.Pipeline)
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'contacts',
        loadComponent: () =>
          import('./features/contacts/pages/contacts-list/contacts-list').then(m => m.ContactsList)
      },
      {
        path: 'contacts/:id',
        loadComponent: () =>
          import('./features/contacts/pages/contact-details/contact-details').then(m => m.ContactDetails)
      },
      {
        path: 'companies',
        loadComponent: () =>
          import('./features/companies/pages/companies-list/companies-list').then(m => m.CompaniesList)
      },
      {
        path: 'companies/:id',
        loadComponent: () =>
          import('./features/companies/pages/company-details/company-details').then(m => m.CompanyDetailsPage)
      },
      {
        path: 'applications',
        loadComponent: () =>
          import('./features/applications/pages/applications-list/applications-list').then(m => m.ApplicationsList)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
