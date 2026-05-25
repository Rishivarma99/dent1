import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize, map, Observable, of, tap, throwError } from 'rxjs';
import { environment } from '../../../environment/environment';
import { ApiResponse } from '../../shared/types/api/api-response';
import { unwrapApiResponse } from '../../shared/utils/api/unwrap-api-response';
import { AuthSessionResponse, AuthUser, LoginRequest, TokenPair } from './auth.types';
import { skipAuth } from './auth-context';
import { TokenStorage } from './token-storage';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokens = inject(TokenStorage);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiUrl}/api/auth`;

  readonly currentUser = signal<AuthUser | null>(null);
  readonly isAuthenticated = computed(() => this.currentUser() !== null);

  login(credentials: LoginRequest): Observable<AuthSessionResponse> {
    return this.http
      .post<ApiResponse<AuthSessionResponse>>(`${this.baseUrl}/login`, credentials, skipAuth())
      .pipe(
        map(unwrapApiResponse),
        tap((response) => {
          this.tokens.setTokens(response.accessToken, response.refreshToken);
          this.currentUser.set(response.user);
        })
      );
  }

  refresh(): Observable<TokenPair> {
    const refreshToken = this.tokens.getRefresh();
    if (!refreshToken) {
      return throwError(() => new Error('Missing refresh token'));
    }

    return this.http
      .post<ApiResponse<TokenPair>>(`${this.baseUrl}/refresh`, { refreshToken }, skipAuth())
      .pipe(map(unwrapApiResponse));
  }

  fetchCurrentUser(): Observable<AuthUser> {
    return this.http
      .get<ApiResponse<AuthUser>>(`${this.baseUrl}/me`)
      .pipe(map(unwrapApiResponse));
  }

  logout(): Observable<void> {
    const refreshToken = this.tokens.getRefresh();
    if (!refreshToken) {
      this.hardLogout();
      return of(void 0);
    }

    return this.http
      .post<void>(`${this.baseUrl}/logout`, { refreshToken })
      .pipe(finalize(() => this.hardLogout()));
  }

  hardLogout(): void {
    this.tokens.clear();
    this.currentUser.set(null);
    void this.router.navigate(['/auth/login']);
  }

  hasRole(role: string) {
    return computed(() => this.currentUser()?.roles.includes(role) ?? false);
  }
}
