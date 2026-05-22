using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Data;
using Dent1.Business.Features.Doctors.Models;
using Dent1.Common.MultiTenancy;
using Dent1.Data;
using Dent1.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Features.Doctors.Queries.GetAllDoctors;

public sealed class GetAllDoctorsQueryHandler : IQueryHandler<GetAllDoctorsQuery, List<DoctorReadModel>>
{
    private readonly DentContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public GetAllDoctorsQueryHandler(DentContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public async Task<List<DoctorReadModel>> Handle(GetAllDoctorsQuery query, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsResolved)
        {
            throw new InvalidOperationException("Tenant not resolved.");
        }

        var doctors = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.TenantId == _currentTenant.TenantId && x.Role == UserRole.Doctor && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        return doctors
            .Select(x => new DoctorReadModel(x.Id, x.Name, DoctorSpecialtyRegistry.Get(x.Id)))
            .ToList();
    }
}
