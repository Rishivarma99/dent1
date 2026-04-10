namespace Dent1.Common.Authorization;

/// <summary>
/// Centralized permission catalog.
/// Format: resource.action
/// </summary>
public static class PermissionCodes
{
    // Patient permissions
    public const string PatientRead = "patient.read";
    public const string PatientCreate = "patient.create";
    public const string PatientUpdate = "patient.update";
    public const string PatientDelete = "patient.delete";

    // Appointment permissions
    public const string AppointmentRead = "appointment.read";
    public const string AppointmentCreate = "appointment.create";
    public const string AppointmentUpdate = "appointment.update";
    public const string AppointmentCancel = "appointment.cancel";

    // Prescription permissions
    public const string PrescriptionRead = "prescription.read";
    public const string PrescriptionCreate = "prescription.create";
    public const string PrescriptionUpdate = "prescription.update";

    // User/Staff permissions
    public const string UserRead = "user.read";
    public const string UserCreate = "user.create";
    public const string UserUpdate = "user.update";
    public const string UserManage = "user.manage";

    // Billing permissions
    public const string BillingRead = "billing.read";
    public const string BillingCreate = "billing.create";
    public const string BillingUpdate = "billing.update";

    // Admin permissions
    public const string SettingsManage = "settings.manage";
}

/// <summary>
/// Scope types for fine-grained access control.
/// </summary>
public enum ScopeType
{
    /// <summary>
    /// User can access all resources of this type.
    /// </summary>
    All,

    /// <summary>
    /// User can access within their tenant only.
    /// </summary>
    Tenant,

    /// <summary>
    /// User can access within their branch only.
    /// </summary>
    Branch,

    /// <summary>
    /// User can access within their department only.
    /// </summary>
    Department,

    /// <summary>
    /// User can access resources assigned to them.
    /// </summary>
    Assigned,

    /// <summary>
    /// User can access only their own resources.
    /// </summary>
    Own
}
