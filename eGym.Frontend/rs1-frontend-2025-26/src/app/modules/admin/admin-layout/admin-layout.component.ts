import {
  AfterViewInit,
  Component,
  ElementRef,
  NgZone,
  OnInit,
  PLATFORM_ID,
  ViewChild,
  inject,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs/operators';
import {
  prepareRouteAnimationState,
  routeAnimations,
} from '../../../core/animations/route-animations';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { LayoutResponsiveService } from '../../../core/services/layout-responsive.service';
import { UserProfileService } from '../../../core/services/user-profile.service';

@Component({
  selector: 'app-admin-layout',
  standalone: false,
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
  animations: [routeAnimations],
})
export class AdminLayoutComponent implements OnInit, AfterViewInit {
  @ViewChild('dashMain') private dashMain?: ElementRef<HTMLElement>;
  @ViewChild(RouterOutlet) private outlet?: RouterOutlet;

  auth = inject(AuthFacadeService);
  profileService = inject(UserProfileService);
  private router = inject(Router);
  private layoutResponsive = inject(LayoutResponsiveService);
  private platformId = inject(PLATFORM_ID);
  private ngZone = inject(NgZone);

  readonly isHandset = toSignal(this.layoutResponsive.isHandset$, { initialValue: false });
  sidenavOpen = false;
  animationState = '';

  /** Disabled during SSR + first client paint to kill the F5 hang/blink. */
  animationsDisabled = true;

  ngOnInit(): void {
    this.profileService.loadProfile().subscribe();
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        this.syncRouteAnimationState();
        this.scrollMainToTop();
      });

    if (isPlatformBrowser(this.platformId)) {
      this.ngZone.runOutsideAngular(() => {
        setTimeout(() => (this.animationsDisabled = false), 100);
      });
    }
  }

  ngAfterViewInit(): void {
    this.syncRouteAnimationState();
  }

  onRouteActivate(): void {
    this.syncRouteAnimationState();
  }

  toggleSidenav(): void {
    this.sidenavOpen = !this.sidenavOpen;
  }

  closeSidenavIfHandset(): void {
    if (this.isHandset()) {
      this.sidenavOpen = false;
    }
  }

  logout(): void {
    this.profileService.clear();
    this.router.navigate(['/auth/logout']);
  }

  private syncRouteAnimationState(): void {
    setTimeout(() => {
      this.animationState = prepareRouteAnimationState(this.outlet);
    }, 0);
  }

  private scrollMainToTop(): void {
    const el = this.dashMain?.nativeElement;
    if (el && typeof el.scrollTo === 'function') {
      el.scrollTo(0, 0);
    }
  }
}
