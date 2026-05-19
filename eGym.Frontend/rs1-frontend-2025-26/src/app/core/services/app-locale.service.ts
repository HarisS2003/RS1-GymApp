import { Injectable, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export type AppLanguage = 'bs' | 'en';

export interface LanguageOption {
  code: AppLanguage;
  labelKey: string;
}

@Injectable({ providedIn: 'root' })
export class AppLocaleService {
  private translate = inject(TranslateService);
  private readonly storageKey = 'language';

  readonly languages: LanguageOption[] = [
    { code: 'bs', labelKey: 'COMMON.LANG.BS' },
    { code: 'en', labelKey: 'COMMON.LANG.EN' },
  ];

  currentLang = signal<AppLanguage>('bs');

  init(): void {
    this.translate.addLangs(['en', 'bs']);
    this.translate.setDefaultLang('bs');

    const saved = (localStorage.getItem(this.storageKey) as AppLanguage) || 'bs';
    this.setLanguage(saved, false);
  }

  setLanguage(lang: AppLanguage, persist = true): void {
    this.currentLang.set(lang);
    if (persist) {
      localStorage.setItem(this.storageKey, lang);
    }
    this.translate.use(lang).subscribe();
  }
}
