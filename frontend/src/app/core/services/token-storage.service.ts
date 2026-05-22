import { Injectable } from '@angular/core';
import { StoredAuthSession } from '../models/stored-auth-session';

const ACCESS_TOKEN_KEY = 'dent1_access_token';
const REFRESH_TOKEN_KEY = 'dent1_refresh_token';
const USER_ID_KEY = 'dent1_user_id';
const ROLE_KEY = 'dent1_role';
const ACCESS_TOKEN_EXPIRES_AT_UTC_KEY = 'dent1_access_token_expires_at_utc';
const PERMISSIONS_KEY = 'dent1_permissions';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  saveSession(session: StoredAuthSession): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, session.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, session.refreshToken);
    localStorage.setItem(USER_ID_KEY, session.userId);
    localStorage.setItem(ROLE_KEY, session.role);
    localStorage.setItem(ACCESS_TOKEN_EXPIRES_AT_UTC_KEY, session.accessTokenExpiresAtUtc);
    localStorage.setItem(PERMISSIONS_KEY, JSON.stringify(session.permissions));
  }

  getPermissions(): readonly string[] {
    const raw = localStorage.getItem(PERMISSIONS_KEY);
    if (!raw) {
      return [];
    }

    try {
      const parsed: unknown = JSON.parse(raw);
      return Array.isArray(parsed)
        ? parsed.filter((item): item is string => typeof item === 'string')
        : [];
    } catch {
      return [];
    }
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  getRole(): string | null {
    return localStorage.getItem(ROLE_KEY);
  }

  clear(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_ID_KEY);
    localStorage.removeItem(ROLE_KEY);
    localStorage.removeItem(ACCESS_TOKEN_EXPIRES_AT_UTC_KEY);
    localStorage.removeItem('dent1_access_token_expires_in_seconds');
    localStorage.removeItem(PERMISSIONS_KEY);
  }
}
