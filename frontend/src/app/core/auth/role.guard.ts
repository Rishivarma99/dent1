import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const roleGuard = (roles: string[]): CanActivateFn => () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const user = auth.currentUser();

  return (
    (user !== null && roles.some((role) => user.roles.includes(role))) ||
    router.createUrlTree(['/forbidden'])
  );
};
