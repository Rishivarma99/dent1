import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { isClinicalRole } from '../constants/clinic-roles';
import { TokenStorageService } from '../services/token-storage.service';

export const clinicalRoleGuard: CanActivateFn = () => {
  const role = inject(TokenStorageService).getRole();
  if (isClinicalRole(role)) {
    return true;
  }
  return inject(Router).createUrlTree(['/dashboard']);
};
