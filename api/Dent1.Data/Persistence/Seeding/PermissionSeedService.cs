using Dent1.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories.Seeding;

public interface IPermissionSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class PermissionSeedService : IPermissionSeedService
{
    private readonly DentContext _dbContext;

    public PermissionSeedService(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var permissions = new[]
        {
            new Permission { Id = SeedIds.PatientReadPermissionId, Code = "patient.read", Name = "Read Patients", Module = "Patient", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.PatientCreatePermissionId, Code = "patient.create", Name = "Create Patient", Module = "Patient", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.PatientUpdatePermissionId, Code = "patient.update", Name = "Update Patient", Module = "Patient", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.AppointmentReadPermissionId, Code = "appointment.read", Name = "Read Appointments", Module = "Appointment", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.AppointmentCreatePermissionId, Code = "appointment.create", Name = "Create Appointment", Module = "Appointment", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.AppointmentUpdatePermissionId, Code = "appointment.update", Name = "Update Appointment", Module = "Appointment", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.PrescriptionReadPermissionId, Code = "prescription.read", Name = "Read Prescriptions", Module = "Prescription", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.PrescriptionCreatePermissionId, Code = "prescription.create", Name = "Create Prescription", Module = "Prescription", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.UserReadPermissionId, Code = "user.read", Name = "Read Users", Module = "User", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Permission { Id = SeedIds.UserManagePermissionId, Code = "user.manage", Name = "Manage Users", Module = "User", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };

        foreach (var seed in permissions)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == seed.Id, cancellationToken);
            if (permission is null)
            {
                _dbContext.Permissions.Add(seed);
                continue;
            }

            permission.Code = seed.Code;
            permission.Name = seed.Name;
            permission.Module = seed.Module;
            permission.IsActive = seed.IsActive;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
