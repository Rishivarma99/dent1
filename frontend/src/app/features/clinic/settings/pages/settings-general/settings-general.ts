import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ThemeService } from '../../../../../core/services/theme.service';

@Component({
  selector: 'app-settings-general',
  imports: [ButtonModule],
  templateUrl: './settings-general.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SettingsGeneralPage {
  protected readonly themeService = inject(ThemeService);

  protected readonly isDarkMode = computed(() => this.themeService.theme() === 'dark');

  protected onToggleTheme(): void {
    this.themeService.toggleTheme();
  }
}
