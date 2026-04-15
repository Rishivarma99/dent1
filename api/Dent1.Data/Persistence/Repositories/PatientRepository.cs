using Dent1.Data.Entities;
using Dent1.Data.Interfaces;
using Dent1.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly DentContext _context;
    private readonly ICurrentTenant _currentTenant;

    public PatientRepository(DentContext context, ICurrentTenant currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<List<Patient>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.TenantId == _currentTenant.TenantId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.TenantId == _currentTenant.TenantId)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Patient>> SearchByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.TenantId == _currentTenant.TenantId && p.Phone == phone)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(string name, string phone, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            TenantId = _currentTenant.TenantId,
            Name = name,
            Phone = phone,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Patients.AddAsync(patient, cancellationToken);

        return patient.Id;
    }
}
