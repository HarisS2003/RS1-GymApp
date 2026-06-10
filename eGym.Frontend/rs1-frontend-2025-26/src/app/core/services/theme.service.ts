import { Injectable, PLATFORM_ID, computed, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type AppTheme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly storageKey = 'app-theme';
  private readonly _theme = signal<AppTheme>('light');
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  theme = this._theme.asReadonly();
  isDark = computed(() => this._theme() === 'dark');

  init(): void {
    if (!this.isBrowser) return;
    const saved = localStorage.getItem(this.storageKey) as AppTheme | null;
    this.apply(saved === 'dark' ? 'dark' : 'light');
  }

  toggle(): void {
    this.apply(this._theme() === 'light' ? 'dark' : 'light');
  }

  setTheme(theme: AppTheme): void {
    this.apply(theme);
  }

  private apply(theme: AppTheme): void {
    this._theme.set(theme);
    if (!this.isBrowser) return;
    localStorage.setItem(this.storageKey, theme);

    const root = document.documentElement;
    const body = document.body;
    const allThemeClasses = [
      'theme-light',
      'theme-dark',
      'light',
      'dark',
      'light-theme',
      'dark-theme',
    ];

    root.classList.remove(...allThemeClasses);
    body.classList.remove(...allThemeClasses);

    if (theme === 'dark') {
      root.classList.add('theme-dark', 'dark', 'dark-theme');
      body.classList.add('theme-dark', 'dark', 'dark-theme');
    } else {
      root.classList.add('theme-light', 'light', 'light-theme');
      body.classList.add('theme-light', 'light', 'light-theme');
    }

    body.style.colorScheme = theme;
  }
}
