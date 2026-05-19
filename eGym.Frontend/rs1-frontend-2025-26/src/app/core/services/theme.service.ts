import { Injectable, computed, signal } from '@angular/core';

export type AppTheme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly storageKey = 'app-theme';
  private readonly _theme = signal<AppTheme>('light');

  theme = this._theme.asReadonly();
  isDark = computed(() => this._theme() === 'dark');

  init(): void {
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
    localStorage.setItem(this.storageKey, theme);

    const root = document.documentElement;
    root.classList.remove('theme-light', 'theme-dark');
    root.classList.add(`theme-${theme}`);
    document.body.classList.remove('theme-light', 'theme-dark');
    document.body.classList.add(`theme-${theme}`);
    document.body.style.colorScheme = theme;
  }
}
