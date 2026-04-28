import { Routes } from '@angular/router';

export const PATIENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/patients-page/patients-page').then((m) => m.PatientsPage)
  }
];
