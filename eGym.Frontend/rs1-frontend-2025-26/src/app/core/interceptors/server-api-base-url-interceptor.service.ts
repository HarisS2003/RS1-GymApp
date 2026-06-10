import { HttpInterceptorFn } from '@angular/common/http';
import { PLATFORM_ID, inject } from '@angular/core';
import { isPlatformServer } from '@angular/common';
import { EMPTY } from 'rxjs';

/**
 * SSR request hard-block (allowlist strategy).
 *
 * The Node SSR server cannot reach the .NET backend (CORS / local network), and
 * relative API URLs like `/Gyms` resolve to the SSR server itself
 * (http://localhost:4000/Gyms) which returns index.html → "Http failure during
 * parsing" (HTML parsed as JSON).
 *
 * Fix: on the server, short-circuit with EMPTY for EVERYTHING except i18n /
 * static assets. No request reaches :4000 or :5177 → no CORS, no HTML-as-JSON
 * parse error, no console flood. Components render empty during SSR and the
 * browser refetches real JSON in ngOnInit after hydration (port 4200 proxy).
 */
const SSR_ALLOW_PREFIXES = ['/i18n', '/assets'];
const STATIC_ASSET_RE = /\.(json|js|mjs|css|png|jpe?g|gif|svg|ico|webp|woff2?|ttf|eot|map)(\?|$)/i;

function isAllowedOnServer(url: string): boolean {
  const path = url.replace(/^https?:\/\/[^/]+/i, '') || '/';
  if (SSR_ALLOW_PREFIXES.some((p) => path === p || path.startsWith(`${p}/`))) {
    return true;
  }
  return STATIC_ASSET_RE.test(path);
}

export const serverApiBaseUrlInterceptor: HttpInterceptorFn = (req, next) => {
  const isServer = isPlatformServer(inject(PLATFORM_ID));

  if (isServer && !isAllowedOnServer(req.url)) {
    return EMPTY;
  }

  return next(req);
};
