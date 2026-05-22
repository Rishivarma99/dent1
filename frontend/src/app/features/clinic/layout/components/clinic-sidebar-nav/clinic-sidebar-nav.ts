import { BreakpointObserver } from '@angular/cdk/layout';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  computed,
  inject
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TooltipModule } from 'primeng/tooltip';
import { map } from 'rxjs';
import { ClinicNavConfigService } from '../../../../../core/services/clinic-nav-config.service';
import { TokenStorageService } from '../../../../../core/services/token-storage.service';
import { ClinicSidebarNavIcon } from './clinic-sidebar-nav-icon';

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
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly navConfig = inject(ClinicNavConfigService);

  @Input() isOpen = false;

  @Output() readonly toggleSidebar = new EventEmitter<void>();
  @Output() readonly closeSidebar = new EventEmitter<void>();

  private readonly role = computed(() => this.tokenStorage.getRole());

  protected readonly homeLink = computed(() =>
    this.navConfig.getDefaultLandingPath(this.role())
  );

  protected readonly menuItems = computed(() =>
    this.navConfig.getSidebarItems(this.role())
  );

  /** Tooltips only on tablet icon-only rail (640px–1023px, collapsed). */
  protected readonly isTabletRail = toSignal(
    this.breakpointObserver
      .observe('(min-width: 640px) and (max-width: 1023.98px)')
      .pipe(map(state => state.matches)),
    { initialValue: false }
  );

  protected onToggle(): void {
    this.toggleSidebar.emit();
  }

  protected onClose(): void {
    this.closeSidebar.emit();
  }
}
