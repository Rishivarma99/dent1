import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { MenuModule } from 'primeng/menu';

@Component({
  selector: 'app-sidebar',
  imports: [MenuModule],
  templateUrl: './clinic-sidebar-nav.html',
  styleUrl: './clinic-sidebar-nav.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicSidebarNav {
  protected readonly menuItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-th-large', routerLink: '/dashboard' },
    { label: 'Patients', icon: 'pi pi-users', routerLink: '/patients' },
    { label: 'Doctors', icon: 'pi pi-user-md', routerLink: '/doctors' },
    { label: 'Settings', icon: 'pi pi-cog', routerLink: '/auth/login' }
  ];
}
