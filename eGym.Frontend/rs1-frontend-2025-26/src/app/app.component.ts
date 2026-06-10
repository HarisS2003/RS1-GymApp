import { Component, NgZone, OnInit, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {
  prepareRouteAnimationState,
  routeAnimations,
} from './core/animations/route-animations';
import { AppLocaleService } from './core/services/app-locale.service';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss',
  animations: [routeAnimations],
})
export class AppComponent implements OnInit {
  private locale = inject(AppLocaleService);
  private theme = inject(ThemeService);
  private platformId = inject(PLATFORM_ID);
  private ngZone = inject(NgZone);

  /** Disabled during SSR + first client paint to kill the F5 hang/blink. */
  animationsDisabled = true;

  ngOnInit(): void {
    this.locale.init();
    this.theme.init();

    if (isPlatformBrowser(this.platformId)) {
      this.ngZone.runOutsideAngular(() => {
        setTimeout(() => (this.animationsDisabled = false), 100);
      });
    }
  }

  prepareRoute(outlet: RouterOutlet): string {
    return prepareRouteAnimationState(outlet);
  }
}
