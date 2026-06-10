import {
  AngularNodeAppEngine,
  createNodeRequestHandler,
  isMainModule,
  writeResponseToNodeResponse,
} from '@angular/ssr/node';
import express from 'express';
import { join } from 'node:path';

const browserDistFolder = join(import.meta.dirname, '../browser');

const app = express();
const angularApp = new AngularNodeAppEngine();

/**
 * Reverse proxy for backend API calls.
 *
 * The browser is served from this Node server (e.g. http://localhost:4000),
 * so relative API requests like `/Gyms` would otherwise hit this server and
 * receive index.html (HTML-as-JSON parse error). proxy.conf.json only applies
 * to `ng serve`, not the built server. Here we forward the known backend
 * prefixes to the .NET backend server-to-server — no browser CORS involved.
 */
const BACKEND_URL = (process.env['BACKEND_URL'] || 'http://localhost:5177').replace(/\/$/, '');

function isApiPath(path: string): boolean {
  return path === '/api' || path.startsWith('/api/');
}

app.use(async (req, res, next) => {
  if (!isApiPath(req.path)) {
    next();
    return;
  }

  try {
    const headers: Record<string, string> = {};
    for (const [key, value] of Object.entries(req.headers)) {
      if (value === undefined) continue;
      const lower = key.toLowerCase();
      if (lower === 'host' || lower === 'connection' || lower === 'content-length') continue;
      headers[key] = Array.isArray(value) ? value.join(',') : value;
    }

    let body: Buffer | undefined;
    if (req.method !== 'GET' && req.method !== 'HEAD') {
      body = await new Promise<Buffer>((resolve, reject) => {
        const chunks: Buffer[] = [];
        req.on('data', (c) => chunks.push(c as Buffer));
        req.on('end', () => resolve(Buffer.concat(chunks)));
        req.on('error', reject);
      });
    }

    const backendRes = await fetch(`${BACKEND_URL}${req.originalUrl}`, {
      method: req.method,
      headers,
      body: body ? new Uint8Array(body) : undefined,
      redirect: 'manual',
    });

    res.status(backendRes.status);
    backendRes.headers.forEach((value, key) => {
      const lower = key.toLowerCase();
      if (lower === 'content-encoding' || lower === 'transfer-encoding') return;
      res.setHeader(key, value);
    });
    res.send(Buffer.from(await backendRes.arrayBuffer()));
  } catch (error) {
    console.error('[SSR proxy] backend fetch failed:', (error as Error).message);
    res.status(502).json({ error: 'Backend unavailable' });
  }
});

/**
 * Serve static files from /browser
 */
app.use(
  express.static(browserDistFolder, {
    maxAge: '1y',
    index: false,
    redirect: false,
  }),
);

/**
 * Handle all other requests by rendering the Angular application.
 */
app.use((req, res, next) => {
  angularApp
    .handle(req)
    .then((response) =>
      response ? writeResponseToNodeResponse(response, res) : next(),
    )
    .catch(next);
});

/**
 * Start the server if this module is the main entry point, or it is ran via PM2.
 * The server listens on the port defined by the `PORT` environment variable, or defaults to 4000.
 */
if (isMainModule(import.meta.url) || process.env['pm_id']) {
  const port = process.env['PORT'] || 4000;
  app.listen(port, (error) => {
    if (error) {
      throw error;
    }

    console.log(`Node Express server listening on http://localhost:${port}`);
  });
}

/**
 * Request handler used by the Angular CLI (for dev-server and during build) or Firebase Cloud Functions.
 */
export const reqHandler = createNodeRequestHandler(app);
