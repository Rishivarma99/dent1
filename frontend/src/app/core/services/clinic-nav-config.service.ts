import { Injectable } from '@angular/core';
import { getDefaultLandingPath, isClinicalRole } from '../constants/clinic-roles';
import { ClinicSidebarIconId } from '../../features/clinic/layout/components/clinic-sidebar-nav/clinic-sidebar-icons';

export interface ClinicSidebarNavItem {
  readonly label: string;
  readonly icon: ClinicSidebarIconId;
  readonly routerLink: string;
  readonly exact?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ClinicNavConfigService {
  getDefaultLandingPath(role: string | null | undefined): string {
    return getDefaultLandingPath(role);
  }

  getSidebarItems(role: string | null | undefined): readonly ClinicSidebarNavItem[] {
    if (isClinicalRole(role)) {
      return [
        { label: 'My Workspace', icon: 'workspace', routerLink: '/workspace', exact: true },
        { label: 'Patients', icon: 'patients', routerLink: '/patients' },
        { label: 'Appointments', icon: 'appointments', routerLink: '/appointments' },
        { label: 'Settings', icon: 'settings', routerLink: '/settings' }
      ];
    }

    return [
      { label: 'Dashboard', icon: 'dashboard', routerLink: '/dashboard', exact: true },
      { label: 'Patients', icon: 'patients', routerLink: '/patients' },
      { label: 'Appointments', icon: 'appointments', routerLink: '/appointments' },
      { label: 'Settings', icon: 'settings', routerLink: '/settings' }
    ];
  }
}
