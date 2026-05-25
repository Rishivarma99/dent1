import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TokenStorage {
  private readonly accessKey = 'auth.access';
  private readonly refreshKey = 'auth.refresh';

  getAccess(): string | null {
    return sessionStorage.getItem(this.accessKey);
  }

  getRefresh(): string | null {
    return sessionStorage.getItem(this.refreshKey);
  }

  setTokens(accessToken: string, refreshToken: string): void {
    sessionStorage.setItem(this.accessKey, accessToken);
    sessionStorage.setItem(this.refreshKey, refreshToken);
  }

  clear(): void {
    sessionStorage.removeItem(this.accessKey);
    sessionStorage.removeItem(this.refreshKey);
  }
}
