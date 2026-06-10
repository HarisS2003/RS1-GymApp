export const environment = {
  production: false,
  // Browser: relative `/api` → dev-server proxy (proxy.conf.json) / SSR express
  // proxy forwards to backend (api/[controller]) → no CORS.
  apiUrl: '/api',
  ssrApiUrl: 'http://localhost:5177'
};
