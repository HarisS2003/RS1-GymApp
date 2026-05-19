import { Component, computed, inject, Input } from '@angular/core';
import { AppLocaleService, AppLanguage } from '../../../../core/services/app-locale.service';
import { ThemeService } from '../../../../core/services/theme.service';

@Component({
  selector: 'app-locale-theme-toolbar',
  standalone: false,
  templateUrl: './locale-theme-toolbar.component.html',
  styleUrl: './locale-theme-toolbar.component.scss',
})
export class LocaleThemeToolbarComponent {
  @Input() compact = false;

  locale = inject(AppLocaleService);
  theme = inject(ThemeService);

  currentLangLabelKey = computed(() => {
    const code = this.locale.currentLang();
    return this.locale.languages.find((l) => l.code === code)?.labelKey ?? 'COMMON.LANG.BS';
  });

  setLanguage(lang: AppLanguage): void {
    this.locale.setLanguage(lang);
  }

  toggleTheme(): void {
    this.theme.toggle();
  }
}
