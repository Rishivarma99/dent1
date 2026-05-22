import { Routes } from '@angular/router';
import { opsRoleGuard } from '../../../core/guards/ops-role.guard';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    canActivate: [opsRoleGuard],
    data: { headerTitle: 'Dashboard' },
    loadComponent: () => import('./pages/dashboard-page/dashboard-page').then((m) => m.DashboardPage)
  }
];
