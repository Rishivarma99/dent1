import { Routes } from '@angular/router';
import { ClinicLayoutComponent } from './layout/clinic-layout.component';
import { ClinicLandingRedirectComponent } from './layout/clinic-landing-redirect.component';

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
        path: 'workspace',
        loadChildren: () => import('./workspace/routes').then((m) => m.WORKSPACE_ROUTES)
      },
      {
        path: 'patients',
        loadChildren: () => import('./patients/routes').then((m) => m.PATIENT_ROUTES)
      },
      {
        path: 'appointments',
        loadChildren: () => import('./appointments/routes').then((m) => m.APPOINTMENTS_ROUTES)
      },
      {
        path: 'settings',
        loadChildren: () => import('./settings/routes').then((m) => m.SETTINGS_ROUTES)
      },
      {
        path: 'doctors',
        pathMatch: 'full',
        redirectTo: 'settings/staff'
      },
      {
        path: '',
        pathMatch: 'full',
        component: ClinicLandingRedirectComponent
      }
    ]
  }
];
