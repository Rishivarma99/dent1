import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Auth, signOut } from '@angular/fire/auth';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly router = inject(Router);
  private readonly firebaseAuth = inject(Auth);

  logout(): void {
    this.tokenStorage.clear();
    void signOut(this.firebaseAuth).catch(() => undefined);
    void this.router.navigate(['/auth/login']);
  }

  hasSession(): boolean {
    return !!this.tokenStorage.getAccessToken();
  }
}
