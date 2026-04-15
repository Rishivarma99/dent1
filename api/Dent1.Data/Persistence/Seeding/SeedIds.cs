namespace Dent1.Data.Repositories.Seeding;

internal static class SeedIds
{
    public static readonly Guid DefaultTenantId = Guid.Parse("b1b2c3d4-0001-0000-0000-000000000001");

    public static readonly Guid AdminRoleId = Guid.Parse("10000000-0001-0000-0000-000000000001");
    public static readonly Guid DoctorRoleId = Guid.Parse("10000000-0002-0000-0000-000000000001");
    public static readonly Guid ReceptionistRoleId = Guid.Parse("10000000-0003-0000-0000-000000000001");
    public static readonly Guid AssistantRoleId = Guid.Parse("10000000-0004-0000-0000-000000000001");
    public static readonly Guid PatientRoleId = Guid.Parse("10000000-0005-0000-0000-000000000001");

    public static readonly Guid PatientReadPermissionId = Guid.Parse("20000000-0001-0000-0000-000000000001");
    public static readonly Guid PatientCreatePermissionId = Guid.Parse("20000000-0002-0000-0000-000000000001");
    public static readonly Guid PatientUpdatePermissionId = Guid.Parse("20000000-0003-0000-0000-000000000001");
    public static readonly Guid AppointmentReadPermissionId = Guid.Parse("20000000-0004-0000-0000-000000000001");
    public static readonly Guid AppointmentCreatePermissionId = Guid.Parse("20000000-0005-0000-0000-000000000001");
    public static readonly Guid AppointmentUpdatePermissionId = Guid.Parse("20000000-0006-0000-0000-000000000001");
    public static readonly Guid PrescriptionReadPermissionId = Guid.Parse("20000000-0007-0000-0000-000000000001");
    public static readonly Guid PrescriptionCreatePermissionId = Guid.Parse("20000000-0008-0000-0000-000000000001");
    public static readonly Guid UserReadPermissionId = Guid.Parse("20000000-0009-0000-0000-000000000001");
    public static readonly Guid UserManagePermissionId = Guid.Parse("20000000-0010-0000-0000-000000000001");
}
