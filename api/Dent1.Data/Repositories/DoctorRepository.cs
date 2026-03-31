using Dent1.Data.Entities;
using Dent1.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly DentContext _context;

    public DoctorRepository(DentContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(string name, string specialty, CancellationToken cancellationToken)
    {
        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            Name = name,
            Specialty = specialty
        };

        await _context.Doctors.AddAsync(doctor, cancellationToken);
        return doctor.Id;
    }

    public async Task<List<Doctor>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Doctors
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string specialty, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doctor is null)
        {
            return false;
        }

        doctor.Name = name;
        doctor.Specialty = specialty;
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doctor is null)
        {
            return false;
        }

        _context.Doctors.Remove(doctor);
        return true;
    }
}
