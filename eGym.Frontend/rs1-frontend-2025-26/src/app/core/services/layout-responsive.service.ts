import { BreakpointObserver } from '@angular/cdk/layout';
import { Injectable, inject } from '@angular/core';
import { map, shareReplay } from 'rxjs/operators';

/** Handset / narrow layout — matches dashboard `@media (max-width: 900px)`. */
@Injectable({ providedIn: 'root' })
export class LayoutResponsiveService {
  private readonly breakpointObserver = inject(BreakpointObserver);

  readonly isHandset$ = this.breakpointObserver.observe('(max-width: 900px)').pipe(
    map((state) => state.matches),
    shareReplay({ bufferSize: 1, refCount: true }),
  );
}
