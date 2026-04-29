import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TooltipModule } from 'primeng/tooltip';

interface SidebarMenuItem {
  readonly label: string;
  readonly icon: string;
  readonly routerLink: string;
}

@Component({
  selector: 'app-sidebar',
  imports: [NgClass, RouterLink, RouterLinkActive, TooltipModule],
  templateUrl: './clinic-sidebar-nav.html',
  styleUrl: './clinic-sidebar-nav.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.clinic-sidebar-host--open]': 'isOpen'
  }
})
export class ClinicSidebarNav {
  @Input() isOpen = false;

  @Output() readonly toggleSidebar = new EventEmitter<void>();
  @Output() readonly closeSidebar = new EventEmitter<void>();

  protected readonly menuItems: readonly SidebarMenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-th-large', routerLink: '/dashboard' },
    { label: 'Patients', icon: 'pi pi-users', routerLink: '/patients' },
    { label: 'Doctors', icon: 'pi pi-user-md', routerLink: '/doctors' },
    { label: 'Settings', icon: 'pi pi-cog', routerLink: '/auth/login' }
  ];

  protected onToggle(): void {
    this.toggleSidebar.emit();
  }

  protected onClose(): void {
    this.closeSidebar.emit();
  }
}
