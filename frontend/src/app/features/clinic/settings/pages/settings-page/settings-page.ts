import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ThemeService } from '../../../../../core/services/theme.service';

@Component({
  selector: 'app-settings-page',
  imports: [ButtonModule],
  templateUrl: './settings-page.html',
  styleUrl: './settings-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SettingsPage {
  protected readonly themeService = inject(ThemeService);

  protected readonly isDarkMode = computed(() => this.themeService.theme() === 'dark');

  protected onToggleTheme(): void {
    this.themeService.toggleTheme();
  }
}
