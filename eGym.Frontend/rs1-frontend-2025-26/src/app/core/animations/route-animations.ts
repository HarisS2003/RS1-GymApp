import {
  animate,
  group,
  query,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { RouterOutlet } from '@angular/router';

const ROUTE_ANIM_MS = 300;
const ROUTE_ANIM_TIMING = `${ROUTE_ANIM_MS}ms ease-in-out`;

/** Route `data.animation` helper — use in route configs. */
export function withPageAnimation(animation: string): { animation: string } {
  return { animation };
}

/**
 * Cross-fade between sibling routes (root + layout router-outlets).
 * Requires `.route-outlet-host` (CSS grid stack + overflow hidden).
 * Bind: [@routeAnimations]="prepareRouteAnimationState(outlet)"
 */
export const routeAnimations = trigger('routeAnimations', [
  transition('* <=> *', [
    query(':enter', [style({ opacity: 0, zIndex: 1 })], { optional: true }),
    query(
      ':leave',
      [style({ opacity: 1, zIndex: 0, pointerEvents: 'none' })],
      { optional: true },
    ),
    group([
      query(
        ':leave',
        [animate(ROUTE_ANIM_TIMING, style({ opacity: 0 }))],
        { optional: true },
      ),
      query(
        ':enter',
        [animate(ROUTE_ANIM_TIMING, style({ opacity: 1 }))],
        { optional: true },
      ),
    ]),
  ]),
]);

export function prepareRouteAnimationState(outlet: RouterOutlet | null | undefined): string {
  if (!outlet?.isActivated) {
    return 'void';
  }

  const animation = outlet.activatedRouteData?.['animation'];
  if (typeof animation === 'string' && animation.length > 0) {
    return animation;
  }

  const path = outlet.activatedRoute.snapshot.url.map((segment) => segment.path).join('/');
  return path || 'root';
}
