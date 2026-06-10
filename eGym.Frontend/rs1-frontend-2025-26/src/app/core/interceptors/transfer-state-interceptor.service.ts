import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { PLATFORM_ID, StateKey, TransferState, inject, makeStateKey } from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { EMPTY, of, catchError, tap } from 'rxjs';

/**
 * SSR ⇄ browser GET caching + graceful SSR failure.
 *
 * - Server: try the request. On success cache the body in TransferState so the
 *   browser reuses it (no duplicate fetch). On failure (CORS/local network block
 *   between Node :4000 and .NET :5177) swallow with EMPTY — the app does NOT
 *   crash, it just renders empty and the browser refetches in ngOnInit.
 * - Browser: if a cached SSR body exists, return it once (then drop it to avoid
 *   stale duplicates); otherwise fetch normally (via proxy, port 4200 allowed).
 *
 * MUST run before serverApiBaseUrlInterceptor so the cache key uses the original
 * relative URL on both server and browser (keys stay identical).
 */
export const transferStateInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.method !== 'GET') {
    return next(req);
  }

  const transferState = inject(TransferState);
  const isServer = isPlatformServer(inject(PLATFORM_ID));
  const key: StateKey<unknown> = makeStateKey<unknown>(`http:GET:${req.urlWithParams}`);

  if (!isServer) {
    if (transferState.hasKey(key)) {
      const cached = transferState.get<unknown>(key, null);
      transferState.remove(key);
      return of(new HttpResponse({ status: 200, body: cached, url: req.urlWithParams }));
    }
    return next(req);
  }

  return next(req).pipe(
    tap((event) => {
      if (event instanceof HttpResponse) {
        transferState.set(key, event.body);
      }
    }),
    catchError(() => EMPTY),
  );
};
