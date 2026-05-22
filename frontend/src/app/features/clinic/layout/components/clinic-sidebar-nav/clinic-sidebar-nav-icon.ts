import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import {
  CLINIC_SIDEBAR_ICON_PATHS,
  ClinicSidebarIconId
} from './clinic-sidebar-icons';

@Component({
  selector: 'app-clinic-sidebar-nav-icon',
  template: `
    @switch (icon()) {
      @case ('dashboard') {
        <svg class="clinic-sidebar-nav-icon__svg" viewBox="0 0 20 20" fill="none" aria-hidden="true">
          <path
            [attr.d]="paths.dashboard"
            stroke="currentColor"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="1.66667"
          />
        </svg>
      }
      @case ('patients') {
        <svg class="clinic-sidebar-nav-icon__svg" viewBox="0 0 20 20" fill="none" aria-hidden="true">
          <g clip-path="url(#clinic-sidebar-patients-clip)">
            <path
              [attr.d]="paths.patients"
              stroke="currentColor"
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="1.66667"
            />
          </g>
          <defs>
            <clipPath id="clinic-sidebar-patients-clip">
              <rect width="20" height="20" fill="white" />
            </clipPath>
          </defs>
        </svg>
      }
      @case ('settings') {
        <svg class="clinic-sidebar-nav-icon__svg" viewBox="0 0 20 20" fill="none" aria-hidden="true">
          <path
            [attr.d]="paths.settingsOuter"
            stroke="currentColor"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="1.66667"
          />
          <path
            [attr.d]="paths.settingsInner"
            stroke="currentColor"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="1.66667"
          />
        </svg>
      }
      @case ('doctors') {
        <i class="pi pi-user-md clinic-sidebar-nav-icon__prime" aria-hidden="true"></i>
      }
    }
  `,
  styles: `
    :host {
      display: inline-flex;
      flex: 0 0 auto;
      width: 1.25rem;
      height: 1.25rem;
      align-items: center;
      justify-content: center;
    }

    .clinic-sidebar-nav-icon__svg {
      width: 100%;
      height: 100%;
    }

    .clinic-sidebar-nav-icon__prime {
      font-size: 1.125rem;
      line-height: 1;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicSidebarNavIcon {
  readonly icon = input.required<ClinicSidebarIconId>();
  protected readonly paths = CLINIC_SIDEBAR_ICON_PATHS;
}
