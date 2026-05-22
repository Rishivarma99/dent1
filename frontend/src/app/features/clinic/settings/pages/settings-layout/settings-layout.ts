import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CLINIC_ROLE } from '../../../../../core/constants/clinic-roles';
import { TokenStorageService } from '../../../../../core/services/token-storage.service';

@Component({
  selector: 'app-settings-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './settings-layout.html',
  styleUrl: './settings-layout.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SettingsLayoutPage {
  private readonly tokenStorage = inject(TokenStorageService);

  protected readonly showStaffNav = computed(
    () => this.tokenStorage.getRole() === CLINIC_ROLE.Admin
  );
}
