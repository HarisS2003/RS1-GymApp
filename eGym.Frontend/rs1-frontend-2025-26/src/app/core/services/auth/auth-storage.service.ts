import { Injectable, PLATFORM_ID, REQUEST, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import {
  LoginCommandDto,
  RefreshTokenCommandDto
} from '../../../api-services/auth/auth-api.model';

/**
 * Low-level service for managing auth tokens.
 *
 * Hybrid strategy:
 * - Browser: read/write localStorage (primary) and mirror tokens into cookies
 *   so the server can recover auth state on the next full-page render (F5).
 * - Server (SSR): localStorage does not exist, so tokens are read from the
 *   incoming request cookies. This prevents auth-state blinking / loss on refresh.
 *
 * Should not be used directly in components - use AuthFacadeService instead.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthStorageService {
  private readonly ACCESS_TOKEN_KEY = 'accessToken';
  private readonly REFRESH_TOKEN_KEY = 'refreshToken';
  private readonly ACCESS_EXPIRES_KEY = 'accessTokenExpiresAtUtc';
  private readonly REFRESH_EXPIRES_KEY = 'refreshTokenExpiresAtUtc';

  private readonly platformId = inject(PLATFORM_ID);
  private readonly request = inject(REQUEST, { optional: true });
  private readonly isBrowser = isPlatformBrowser(this.platformId);

  /**
   * Save login response (browser only).
   */
  saveLogin(response: LoginCommandDto): void {
    if (!this.isBrowser) return;
    this.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
    this.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    this.setItem(this.ACCESS_EXPIRES_KEY, response.expiresAtUtc);
  }

  /**
   * Save refresh response (browser only).
   */
  saveRefresh(response: RefreshTokenCommandDto): void {
    if (!this.isBrowser) return;
    this.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
    this.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    this.setItem(this.ACCESS_EXPIRES_KEY, response.accessTokenExpiresAtUtc);
    this.setItem(this.REFRESH_EXPIRES_KEY, response.refreshTokenExpiresAtUtc);
  }

  /**
   * Clear all auth data (browser only).
   */
  clear(): void {
    if (!this.isBrowser) return;
    this.removeItem(this.ACCESS_TOKEN_KEY);
    this.removeItem(this.REFRESH_TOKEN_KEY);
    this.removeItem(this.ACCESS_EXPIRES_KEY);
    this.removeItem(this.REFRESH_EXPIRES_KEY);
  }

  /**
   * Get access token. Browser → localStorage, Server → request cookie.
   */
  getAccessToken(): string | null {
    return this.getItem(this.ACCESS_TOKEN_KEY);
  }

  /**
   * Get refresh token. Browser → localStorage, Server → request cookie.
   */
  getRefreshToken(): string | null {
    return this.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Check if user has access token.
   */
  hasToken(): boolean {
    return !!this.getAccessToken();
  }

  // =========================================================
  // PLATFORM-SAFE STORAGE HELPERS
  // =========================================================

  private getItem(key: string): string | null {
    if (this.isBrowser) {
      return localStorage.getItem(key) ?? this.readCookie(key);
    }
    return this.readServerCookie(key);
  }

  private setItem(key: string, value: string): void {
    localStorage.setItem(key, value);
    this.writeCookie(key, value);
  }

  private removeItem(key: string): void {
    localStorage.removeItem(key);
    this.deleteCookie(key);
  }

  // =========================================================
  // COOKIE HELPERS (hybrid SSR fallback)
  // =========================================================

  private writeCookie(key: string, value: string): void {
    if (typeof document === 'undefined') return;
    const maxAge = 60 * 60 * 24 * 30; // 30 days
    document.cookie = `${key}=${encodeURIComponent(value)}; path=/; max-age=${maxAge}; SameSite=Lax`;
  }

  private deleteCookie(key: string): void {
    if (typeof document === 'undefined') return;
    document.cookie = `${key}=; path=/; max-age=0; SameSite=Lax`;
  }

  private readCookie(key: string): string | null {
    if (typeof document === 'undefined') return null;
    return this.parseCookieHeader(document.cookie, key);
  }

  private readServerCookie(key: string): string | null {
    const header = this.request?.headers?.get('cookie') ?? null;
    return this.parseCookieHeader(header, key);
  }

  private parseCookieHeader(header: string | null, key: string): string | null {
    if (!header) return null;
    for (const part of header.split(';')) {
      const [name, ...rest] = part.trim().split('=');
      if (name === key) {
        return decodeURIComponent(rest.join('='));
      }
    }
    return null;
  }
}
