import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Output,
  computed,
  inject,
  signal,
  viewChild
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Menu, MenuModule } from 'primeng/menu';
import { TooltipModule } from 'primeng/tooltip';
import { filter, map, startWith } from 'rxjs';
import { AuthSessionService } from '../../../core/services/auth-session.service';
import { ThemeService } from '../../../core/services/theme.service';
import { TokenStorageService } from '../../../core/services/token-storage.service';
import { log } from 'node:console';

interface HeaderBreadcrumb {
  readonly label: string;
  readonly path: string;
}

const ROUTE_LABELS: Record<string, string> = {
  dashboard: 'Dashboard',
  patients: 'Patients',
  doctors: 'Doctors',
  appointments: 'Appointments',
  visits: 'Visits',
  treatments: 'Treatments',
  reports: 'Reports'
};

@Component({
  selector: 'app-header',
  imports: [RouterLink, ButtonModule, MenuModule, TooltipModule, InputTextModule],
  templateUrl: './app-header.html',
  styleUrl: './app-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppHeaderComponent {
  @Output() readonly openSidebar = new EventEmitter<void>();

  private readonly router = inject(Router);
  private readonly authSession = inject(AuthSessionService);
  private readonly tokenStorage = inject(TokenStorageService);
  protected readonly themeService = inject(ThemeService);

  private readonly profileMenuRef = viewChild<Menu>('profileMenu');
  private readonly searchRef = viewChild<ElementRef<HTMLElement>>('searchRef');

  protected readonly searchExpanded = signal(false);

  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects),
      startWith(this.router.url)
    ),
    { initialValue: this.router.url }
  );

  protected readonly breadcrumbs = computed(() => this.buildBreadcrumbs(this.currentUrl()));

  protected readonly userName = computed(() => this.formatRole(this.tokenStorage.getRole()));
  protected readonly userTitle = computed(() => this.tokenStorage.getRole() ?? 'Staff');
  protected readonly userEmail = 'user@dentova.com';
  protected readonly clinicName = 'Sunrise Dental Clinic';
  protected readonly avatarUrl = 'https://i.pravatar.cc/72?img=12';

  protected readonly profileMenuModel = computed<MenuItem[]>(() => [
    {
      label: 'My Profile',
      icon: 'pi pi-user',
      command: () => undefined
    },
    {
      label: 'Settings',
      icon: 'pi pi-cog',
      command: () => undefined
    },
    { separator: true },
    {
      label: 'Logout',
      icon: 'pi pi-sign-out',
      styleClass: 'app-header-profile-menu-logout',
      command: () => {
        void this.profileMenuRef()?.hide();
        this.authSession.logout();
      }
    }
  ]);

  @HostListener('document:mousedown', ['$event'])
  onDocumentMouseDown(event: MouseEvent): void {
    const target = event.target as Node;

    if (this.searchExpanded() && !this.searchRef()?.nativeElement.contains(target)) {
      this.searchExpanded.set(false);
    }
  } 

  @HostListener('window:resize')
  onWindowResize(): void { 
    this.profileMenuRef()?.hide();
  }

  protected onOpenSidebar(): void {
    this.openSidebar.emit();
  }

  protected onProfileMenuToggle(event: Event): void {
    this.profileMenuRef()?.toggle(event);
  }

  protected onSearchMouseEnter(): void {
    this.searchExpanded.set(true);
  }

  private buildBreadcrumbs(url: string): HeaderBreadcrumb[] {
    const paths = url.split('?')[0].split('/').filter(Boolean);
    const breadcrumbs: HeaderBreadcrumb[] = [{ label: 'Dashboard', path: '/dashboard' }];

    paths.forEach((segment, index) => {
      if (segment === 'dashboard') {
        return;
      }

      const path = `/${paths.slice(0, index + 1).join('/')}`;

      if (ROUTE_LABELS[segment]) {
        breadcrumbs.push({ label: ROUTE_LABELS[segment], path });
        return;
      }

      if (index > 0 && !Number.isNaN(Number(segment))) {
        breadcrumbs.push({ label: `#${segment}`, path });
      }
    });

    return breadcrumbs;
  }

  private formatRole(role: string | null): string {
    if (!role) {
      return 'User';
    }

    return role
      .split(/[\s_-]+/)
      .filter(Boolean)
      .map((part) => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
      .join(' ');
  }
}
