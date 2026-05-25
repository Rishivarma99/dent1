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
import { Location } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, NavigationEnd, Router, RouterLink } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Menu, MenuModule } from 'primeng/menu';
import { TooltipModule } from 'primeng/tooltip';
import { filter, map, startWith } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

type HeaderNavMode = 'hamburger' | 'back';

interface HeaderPageContext {
  readonly title: string;
  readonly navMode: HeaderNavMode;
  readonly backUrl: string | null;
}

interface HeaderBreadcrumb {
  readonly label: string;
  readonly path: string;
}

const MODULE_LABELS: Record<string, string> = {
  dashboard: 'Dashboard',
  patients: 'Patients',
  doctors: 'Doctors',
  settings: 'Settings',
  appointments: 'Appointments',
  visits: 'Visit Workspace',
  treatments: 'Treatment Details',
  reports: 'Reports'
};

const DETAIL_TITLE_BY_MODULE: Record<string, string> = {
  patients: 'Patient Profile',
  doctors: 'Doctor Profile',
  appointments: 'Appointment Details'
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
  private readonly route = inject(ActivatedRoute);
  private readonly location = inject(Location);
  private readonly auth = inject(AuthService);
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

  protected readonly pageContext = computed(() => this.resolvePageContext(this.currentUrl()));

  protected readonly breadcrumbs = computed(() => this.buildBreadcrumbs(this.currentUrl()));

  protected readonly userName = computed(() => this.auth.currentUser()?.name ?? 'User');
  protected readonly userTitle = computed(() =>
    this.formatRole(this.auth.currentUser()?.roles[0] ?? 'Staff')
  );
  protected readonly userEmail = computed(() => this.auth.currentUser()?.email ?? 'user@dentova.com');
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
      command: () => {
        void this.profileMenuRef()?.hide();
        void this.router.navigate(['/settings']);
      }
    },
    {
      label: 'Help',
      icon: 'pi pi-question-circle',
      command: () => undefined
    },
    { separator: true },
    {
      label: 'Logout',
      icon: 'pi pi-sign-out',
      styleClass: 'app-header-profile-menu-logout',
      command: () => {
        void this.profileMenuRef()?.hide();
        this.auth.logout().subscribe();
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

  protected onGoBack(): void {
    const backUrl = this.pageContext().backUrl;
    if (backUrl) {
      void this.router.navigateByUrl(backUrl);
      return;
    }

    this.location.back();
  }

  protected onProfileMenuToggle(event: Event): void {
    this.profileMenuRef()?.toggle(event);
  }

  protected onSearchMouseEnter(): void {
    this.searchExpanded.set(true);
  }

  private resolvePageContext(url: string): HeaderPageContext {
    const segments = url.split('?')[0].split('/').filter(Boolean);
    const rootSegment = segments[0] ?? 'dashboard';

    const routeTitle = this.readRouteHeaderTitle();

    if (segments.length <= 1) {
      return {
        title: routeTitle ?? MODULE_LABELS[rootSegment] ?? this.formatSegmentLabel(rootSegment),
        navMode: 'hamburger',
        backUrl: null
      };
    }

    const parentPath = `/${segments.slice(0, -1).join('/')}`;
    const lastSegment = segments[segments.length - 1];
    const parentSegment = segments[segments.length - 2];

    return {
      title: routeTitle ?? this.resolveDetailTitle(parentSegment, lastSegment),
      navMode: 'back',
      backUrl: parentPath
    };
  }

  private readRouteHeaderTitle(): string | null {
    let child: ActivatedRoute | null = this.route.root;
    let title: string | null = null;

    while (child) {
      const headerTitle = child.snapshot.data['headerTitle'];
      if (typeof headerTitle === 'string' && headerTitle.length > 0) {
        title = headerTitle;
      }
      child = child.firstChild;
    }

    return title;
  }

  private resolveDetailTitle(parentSegment: string, lastSegment: string): string {
    if (MODULE_LABELS[lastSegment]) {
      return MODULE_LABELS[lastSegment];
    }

    if (!Number.isNaN(Number(lastSegment))) {
      if (MODULE_LABELS[parentSegment]) {
        return MODULE_LABELS[parentSegment];
      }

      return DETAIL_TITLE_BY_MODULE[parentSegment] ?? this.formatSegmentLabel(parentSegment);
    }

    return this.formatSegmentLabel(lastSegment);
  }

  private buildBreadcrumbs(url: string): HeaderBreadcrumb[] {
    const paths = url.split('?')[0].split('/').filter(Boolean);
    const routeTitle = this.readRouteHeaderTitle();
    const breadcrumbs: HeaderBreadcrumb[] = [{ label: 'Dashboard', path: '/dashboard' }];

    paths.forEach((segment, index) => {
      if (segment === 'dashboard') {
        return;
      }

      const path = `/${paths.slice(0, index + 1).join('/')}`;
      const isLast = index === paths.length - 1;
      const parentSegment = index > 0 ? paths[index - 1] : '';

      if (MODULE_LABELS[segment]) {
        breadcrumbs.push({ label: MODULE_LABELS[segment], path });
        return;
      }

      if (!Number.isNaN(Number(segment))) {
        const label =
          isLast && routeTitle
            ? routeTitle
            : MODULE_LABELS[parentSegment] ??
              DETAIL_TITLE_BY_MODULE[parentSegment] ??
              `#${segment}`;
        breadcrumbs.push({ label, path });
      }
    });

    return breadcrumbs;
  }

  private formatSegmentLabel(segment: string): string {
    return segment
      .split(/[-_]+/)
      .filter(Boolean)
      .map((part) => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
      .join(' ');
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
