import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    // On-demand SSR (not build-time prerender): routes depend on auth/runtime
    // data, so render per-request. Fixes F5 refresh without crawling the API.
    path: '**',
    renderMode: RenderMode.Server
  }
];
