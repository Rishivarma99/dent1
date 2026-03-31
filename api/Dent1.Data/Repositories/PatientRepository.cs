using Dent1.Data.Entities;
using Dent1.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly DentContext _context;

    public PatientRepository(DentContext context)
    {
        _context = context;
    }

    public async Task<List<Patient>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Patient>> SearchByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.Phone == phone)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> AddAsync(string name, string phone, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            Name = name,
            Phone = phone,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Patients.AddAsync(patient, cancellationToken);

        return patient.Id;
    }
}
