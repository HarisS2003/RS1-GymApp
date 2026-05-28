import { Component, OnInit, inject } from '@angular/core';
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

  ngOnInit(): void {
    this.locale.init();
    this.theme.init();
  }

  prepareRoute(outlet: RouterOutlet): string {
    return prepareRouteAnimationState(outlet);
  }
}
