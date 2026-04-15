using Dent1.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories.Seeding;

public interface IRolePermissionSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class RolePermissionSeedService : IRolePermissionSeedService
{
    private readonly DentContext _dbContext;

    public RolePermissionSeedService(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var pairs = new (Guid RoleId, Guid PermissionId)[]
        {
            (SeedIds.AdminRoleId, SeedIds.PatientReadPermissionId),
            (SeedIds.AdminRoleId, SeedIds.PatientCreatePermissionId),
            (SeedIds.AdminRoleId, SeedIds.PatientUpdatePermissionId),
            (SeedIds.AdminRoleId, SeedIds.AppointmentReadPermissionId),
            (SeedIds.AdminRoleId, SeedIds.AppointmentCreatePermissionId),
            (SeedIds.AdminRoleId, SeedIds.AppointmentUpdatePermissionId),
            (SeedIds.AdminRoleId, SeedIds.PrescriptionReadPermissionId),
            (SeedIds.AdminRoleId, SeedIds.PrescriptionCreatePermissionId),
            (SeedIds.AdminRoleId, SeedIds.UserReadPermissionId),
            (SeedIds.AdminRoleId, SeedIds.UserManagePermissionId),
            (SeedIds.DoctorRoleId, SeedIds.PatientReadPermissionId),
            (SeedIds.DoctorRoleId, SeedIds.AppointmentReadPermissionId),
            (SeedIds.DoctorRoleId, SeedIds.AppointmentUpdatePermissionId),
            (SeedIds.DoctorRoleId, SeedIds.PrescriptionCreatePermissionId),
            (SeedIds.ReceptionistRoleId, SeedIds.PatientReadPermissionId),
            (SeedIds.ReceptionistRoleId, SeedIds.PatientCreatePermissionId),
            (SeedIds.ReceptionistRoleId, SeedIds.AppointmentReadPermissionId),
            (SeedIds.ReceptionistRoleId, SeedIds.AppointmentCreatePermissionId),
            (SeedIds.AssistantRoleId, SeedIds.PatientReadPermissionId),
            (SeedIds.AssistantRoleId, SeedIds.AppointmentReadPermissionId),
            (SeedIds.PatientRoleId, SeedIds.AppointmentReadPermissionId)
        };

        foreach (var pair in pairs)
        {
            var exists = await _dbContext.RolePermissions.AnyAsync(
                rp => rp.RoleId == pair.RoleId && rp.PermissionId == pair.PermissionId,
                cancellationToken);

            if (!exists)
            {
                _dbContext.RolePermissions.Add(new RolePermission
                {
                    RoleId = pair.RoleId,
                    PermissionId = pair.PermissionId
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
