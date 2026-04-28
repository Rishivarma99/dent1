import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ClinicSidebarNav } from './components/clinic-sidebar-nav/clinic-sidebar-nav';
import { ClinicTopHeader } from './components/clinic-top-header/clinic-top-header';
import { AppFooterComponent } from './components/app-footer/app-footer';

@Component({
  selector: 'app-clinic-layout',
  imports: [RouterOutlet, ClinicSidebarNav, ClinicTopHeader, AppFooterComponent],
  templateUrl: './clinic-layout.component.html',
  styleUrl: './clinic-layout.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicLayoutComponent {
  readonly isMobileSidebarOpen = signal(false);
  openMobileSidebar(): void {
    this.isMobileSidebarOpen.set(true);
  }

  closeMobileSidebar(): void {
    this.isMobileSidebarOpen.set(false);
  }

  toggleMobileSidebar(): void {
    this.isMobileSidebarOpen.update(value => !value);
  }
}
