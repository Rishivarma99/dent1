import { Routes } from '@angular/router';
import { adminRoleGuard } from '../../../core/guards/admin-role.guard';

export const SETTINGS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/settings-layout/settings-layout').then((m) => m.SettingsLayoutPage),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'general'
      },
      {
        path: 'general',
        loadComponent: () =>
          import('./pages/settings-general/settings-general').then((m) => m.SettingsGeneralPage)
      },
      {
        path: 'staff',
        canActivate: [adminRoleGuard],
        loadChildren: () => import('../doctors/routes').then((m) => m.DOCTOR_ROUTES)
      }
    ]
  }
];
