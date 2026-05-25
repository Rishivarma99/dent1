import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AuthService } from './auth.service';
import { TokenStorage } from './token-storage';

export function authInit(): Promise<void> {
  const auth = inject(AuthService);
  const tokens = inject(TokenStorage);

  if (!tokens.getAccess()) {
    return Promise.resolve();
  }

  return firstValueFrom(auth.fetchCurrentUser())
    .then((user) => {
      auth.currentUser.set(user);
    })
    .catch(() => {
      auth.hardLogout();
    });
}
