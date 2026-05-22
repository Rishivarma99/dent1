import { Routes } from '@angular/router';
import { clinicalRoleGuard } from '../../../core/guards/clinical-role.guard';

export const WORKSPACE_ROUTES: Routes = [
  {
    path: '',
    canActivate: [clinicalRoleGuard],
    data: { headerTitle: 'My Workspace' },
    loadComponent: () =>
      import('./pages/workspace-page/workspace-page').then((m) => m.WorkspacePage)
  }
];
