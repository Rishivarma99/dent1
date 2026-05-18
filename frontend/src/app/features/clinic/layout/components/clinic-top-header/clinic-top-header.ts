import { ChangeDetectionStrategy, Component, EventEmitter, Output, inject } from '@angular/core';
import { TooltipModule } from 'primeng/tooltip';
import { AuthSessionService } from '../../../../../core/services/auth-session.service';
import { TokenStorageService } from '../../../../../core/services/token-storage.service';

@Component({
  selector: 'app-header',
  imports: [TooltipModule],
  templateUrl: './clinic-top-header.html',
  styleUrl: './clinic-top-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicTopHeader {
  @Output() readonly openSidebar = new EventEmitter<void>();

  private readonly authSession = inject(AuthSessionService);
  private readonly tokenStorage = inject(TokenStorageService);

  protected readonly userName = this.tokenStorage.getRole() ?? 'User';

  protected onOpenSidebar(): void {
    this.openSidebar.emit();
  }

  protected onLogout(): void {
    this.authSession.logout();
  }
}
