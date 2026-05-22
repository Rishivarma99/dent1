import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ClinicNavConfigService } from '../../../core/services/clinic-nav-config.service';
import { TokenStorageService } from '../../../core/services/token-storage.service';

@Component({
  selector: 'app-clinic-landing-redirect',
  template: '',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicLandingRedirectComponent {
  private readonly router = inject(Router);
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly navConfig = inject(ClinicNavConfigService);

  constructor() {
    const path = this.navConfig.getDefaultLandingPath(this.tokenStorage.getRole());
    void this.router.navigateByUrl(path, { replaceUrl: true });
  }
}
