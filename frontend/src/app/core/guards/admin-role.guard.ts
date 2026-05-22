import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CLINIC_ROLE } from '../constants/clinic-roles';
import { TokenStorageService } from '../services/token-storage.service';

export const adminRoleGuard: CanActivateFn = () => {
  const role = inject(TokenStorageService).getRole();
  if (role === CLINIC_ROLE.Admin) {
    return true;
  }
  return inject(Router).createUrlTree(['/settings']);
};
