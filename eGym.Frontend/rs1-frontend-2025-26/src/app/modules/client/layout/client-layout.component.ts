import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
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
  selector: 'app-client-layout',
  standalone: false,
  templateUrl: './client-layout.component.html',
  styleUrl: './client-layout.component.scss',
  animations: [routeAnimations],
})
export class ClientLayoutComponent implements OnInit, AfterViewInit {
  @ViewChild('dashMain') private dashMain?: ElementRef<HTMLElement>;
  @ViewChild(RouterOutlet) private outlet?: RouterOutlet;

  auth = inject(AuthFacadeService);
  profileService = inject(UserProfileService);
  private router = inject(Router);
  private layoutResponsive = inject(LayoutResponsiveService);

  profile = this.profileService.profile;
  isTrainer = this.profileService.isTrainer;
  readonly isHandset = toSignal(this.layoutResponsive.isHandset$, { initialValue: false });

  sidenavOpen = false;
  animationState = '';

  ngOnInit(): void {
    this.profileService.loadProfile().subscribe();
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        this.syncRouteAnimationState();
        this.scrollMainToTop();
      });
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
