import { Routes } from '@angular/router';
import { ClinicLayoutComponent } from './layout/clinic-layout.component';

export const CLINIC_ROUTES: Routes = [
  {
    path: '',
    component: ClinicLayoutComponent,
    children: [
      {
        path: 'dashboard',
        loadChildren: () => import('./dashboard/routes').then((m) => m.DASHBOARD_ROUTES)
      },
      {
        path: 'doctors',
        loadChildren: () => import('./doctors/routes').then((m) => m.DOCTOR_ROUTES)
      },
      {
        path: 'patients',
        loadChildren: () => import('./patients/routes').then((m) => m.PATIENT_ROUTES)
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
      }
    ]
  }
];
