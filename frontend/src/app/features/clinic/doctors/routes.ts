import { Routes } from '@angular/router';

export const DOCTOR_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/doctors-page/doctors-page').then((m) => m.DoctorsPage)
  }
];
