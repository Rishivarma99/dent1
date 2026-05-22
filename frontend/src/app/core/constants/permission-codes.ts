/** Mirrors API PermissionCodes until a shared contract exists. */
export const PERMISSION_CODE = {
  appointmentRead: 'appointment.read',
  appointmentCreate: 'appointment.create',
  appointmentUpdate: 'appointment.update',
  prescriptionCreate: 'prescription.create',
  prescriptionUpdate: 'prescription.update'
} as const;
