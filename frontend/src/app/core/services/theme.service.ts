import { DOCUMENT } from '@angular/common';
import { Injectable, effect, inject, signal } from '@angular/core';

const THEME_STORAGE_KEY = 'dent1_theme';

export type AppTheme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);

  readonly theme = signal<AppTheme>(this.readStoredTheme());

  constructor() {
    effect(() => {
      const root = this.document.documentElement;
      const currentTheme = this.theme();

      if (currentTheme === 'dark') {
        root.setAttribute('data-theme', 'dark');
      } else {
        root.removeAttribute('data-theme');
      }

      localStorage.setItem(THEME_STORAGE_KEY, currentTheme);
    });
  }

  toggleTheme(): void {
    this.theme.update((value) => (value === 'light' ? 'dark' : 'light'));
  }

  private readStoredTheme(): AppTheme {
    const stored = localStorage.getItem(THEME_STORAGE_KEY);
    return stored === 'dark' ? 'dark' : 'light';
  }
}
