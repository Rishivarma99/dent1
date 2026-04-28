import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES)
  },
  {
    path: '',
    loadChildren: () => import('./features/clinic/clinic.routes').then((m) => m.CLINIC_ROUTES)
  },
  {
    path: '**',
    redirectTo: 'auth'
  }
];
