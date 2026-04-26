import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { MenuModule } from 'primeng/menu';

@Component({
  selector: 'app-sidebar-nav',
  imports: [MenuModule],
  templateUrl: './app-sidebar-nav.html',
  styleUrl: './app-sidebar-nav.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppSidebarNav {
  protected readonly menuItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-th-large', routerLink: '/patients' },
    { label: 'Patients', icon: 'pi pi-users', routerLink: '/patients' },
    { label: 'Doctors', icon: 'pi pi-user-md', routerLink: '/doctors' },
    { label: 'Settings', icon: 'pi pi-cog', routerLink: '/login' }
  ];
}
