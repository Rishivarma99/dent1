import { BreakpointObserver } from '@angular/cdk/layout';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  inject
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TooltipModule } from 'primeng/tooltip';
import { map } from 'rxjs';
import { ClinicSidebarIconId } from './clinic-sidebar-icons';
import { ClinicSidebarNavIcon } from './clinic-sidebar-nav-icon';

interface SidebarMenuItem {
  readonly label: string;
  readonly icon: ClinicSidebarIconId;
  readonly routerLink: string;
  readonly exact?: boolean;
}

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, TooltipModule, ClinicSidebarNavIcon],
  templateUrl: './clinic-sidebar-nav.html',
  styleUrl: './clinic-sidebar-nav.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.clinic-sidebar-host--open]': 'isOpen'
  }
})
export class ClinicSidebarNav {
  private readonly breakpointObserver = inject(BreakpointObserver);

  @Input() isOpen = false;

  @Output() readonly toggleSidebar = new EventEmitter<void>();
  @Output() readonly closeSidebar = new EventEmitter<void>();

  /** Tooltips only on tablet icon-only rail (640px–1023px, collapsed). */
  protected readonly isTabletRail = toSignal(
    this.breakpointObserver
      .observe('(min-width: 640px) and (max-width: 1023.98px)')
      .pipe(map(state => state.matches)),
    { initialValue: false }
  );

  protected readonly menuItems: readonly SidebarMenuItem[] = [
    { label: 'Dashboard', icon: 'dashboard', routerLink: '/dashboard', exact: true },
    { label: 'Patients', icon: 'patients', routerLink: '/patients' },
    { label: 'Doctors', icon: 'doctors', routerLink: '/doctors' },
    { label: 'Settings', icon: 'settings', routerLink: '/settings' }
  ];

  protected onToggle(): void {
    this.toggleSidebar.emit();
  }

  protected onClose(): void {
    this.closeSidebar.emit();
  }
}
