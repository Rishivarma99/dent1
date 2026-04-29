import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ClinicSidebarNav } from './components/clinic-sidebar-nav/clinic-sidebar-nav';
import { ClinicTopHeader } from './components/clinic-top-header/clinic-top-header';
import { AppFooterComponent } from './components/app-footer/app-footer';

/**
 * isSidebarOpen — single source of truth for sidebar state.
 *   mobile (<640):  true = drawer visible
 *   tablet (640+):  true = sidebar expanded (vs icon-only)
 *   desktop (1024+): true = sidebar expanded (vs icon-only)
 */
@Component({
  selector: 'app-clinic-layout',
  imports: [RouterOutlet, ClinicSidebarNav, ClinicTopHeader, AppFooterComponent],
  templateUrl: './clinic-layout.component.html',
  styleUrl: './clinic-layout.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicLayoutComponent {
  readonly isSidebarOpen = signal(false);

  openSidebar(): void {
    this.isSidebarOpen.set(true);
  }

  closeSidebar(): void {
    this.isSidebarOpen.set(false);
  }

  toggleSidebar(): void {
    this.isSidebarOpen.update(value => !value);
  }
}
