import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { isOpsRole } from '../constants/clinic-roles';
import { TokenStorageService } from '../services/token-storage.service';

export const opsRoleGuard: CanActivateFn = () => {
  const role = inject(TokenStorageService).getRole();
  if (isOpsRole(role)) {
    return true;
  }
  return inject(Router).createUrlTree(['/workspace']);
};
