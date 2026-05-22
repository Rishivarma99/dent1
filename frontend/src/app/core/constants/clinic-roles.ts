/** Matches API `Dent1.Data.Enums.UserRole` string values in JWT / auth response. */
export const CLINIC_ROLE = {
  Doctor: 'Doctor',
  Patient: 'Patient',
  Receptionist: 'Receptionist',
  Assistant: 'Assistant',
  Admin: 'Admin'
} as const;

export type ClinicRole = (typeof CLINIC_ROLE)[keyof typeof CLINIC_ROLE];

const CLINICAL_ROLES: ReadonlySet<string> = new Set([
  CLINIC_ROLE.Doctor,
  CLINIC_ROLE.Assistant
]);

const OPS_ROLES: ReadonlySet<string> = new Set([
  CLINIC_ROLE.Admin,
  CLINIC_ROLE.Receptionist
]);

export function isClinicalRole(role: string | null | undefined): boolean {
  return !!role && CLINICAL_ROLES.has(role);
}

export function isOpsRole(role: string | null | undefined): boolean {
  return !!role && OPS_ROLES.has(role);
}

export function getDefaultLandingPath(role: string | null | undefined): string {
  if (isClinicalRole(role)) {
    return '/workspace';
  }
  if (isOpsRole(role)) {
    return '/dashboard';
  }
  return '/dashboard';
}
