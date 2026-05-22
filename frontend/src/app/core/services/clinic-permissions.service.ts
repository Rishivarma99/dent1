import { Injectable, computed, inject } from '@angular/core';
import { CLINIC_ROLE } from '../constants/clinic-roles';
import { PERMISSION_CODE } from '../constants/permission-codes';
import { TokenStorageService } from './token-storage.service';

export interface ClinicCapabilities {
  readonly canStartVisit: boolean;
  readonly canCompleteVisit: boolean;
  readonly canScheduleFollowUps: boolean;
  readonly canDraftPrescription: boolean;
  readonly canFinalizePrescription: boolean;
}

@Injectable({ providedIn: 'root' })
export class ClinicPermissionsService {
  private readonly tokenStorage = inject(TokenStorageService);

  readonly capabilities = computed(() => {
    const permissions = this.tokenStorage.getPermissions();
    const role = this.tokenStorage.getRole();

    if (permissions.length > 0) {
      return this.resolveFromPermissions(permissions, role);
    }

    return this.resolveFromRole(role);
  });

  private resolveFromPermissions(
    permissions: readonly string[],
    role: string | null
  ): ClinicCapabilities {
    const has = (code: string) => permissions.includes(code);
    const isDoctor = role === CLINIC_ROLE.Doctor;

    return {
      canStartVisit: has(PERMISSION_CODE.appointmentUpdate),
      canCompleteVisit: has(PERMISSION_CODE.appointmentUpdate) && isDoctor,
      canScheduleFollowUps:
        has(PERMISSION_CODE.appointmentCreate) || has(PERMISSION_CODE.appointmentRead),
      canDraftPrescription: has(PERMISSION_CODE.prescriptionCreate),
      canFinalizePrescription:
        has(PERMISSION_CODE.prescriptionUpdate) && isDoctor
    };
  }

  private resolveFromRole(role: string | null): ClinicCapabilities {
    if (role === CLINIC_ROLE.Doctor) {
      return {
        canStartVisit: true,
        canCompleteVisit: true,
        canScheduleFollowUps: true,
        canDraftPrescription: true,
        canFinalizePrescription: true
      };
    }

    if (role === CLINIC_ROLE.Assistant) {
      return {
        canStartVisit: false,
        canCompleteVisit: false,
        canScheduleFollowUps: true,
        canDraftPrescription: true,
        canFinalizePrescription: false
      };
    }

    return {
      canStartVisit: false,
      canCompleteVisit: false,
      canScheduleFollowUps: false,
      canDraftPrescription: false,
      canFinalizePrescription: false
    };
  }
}
