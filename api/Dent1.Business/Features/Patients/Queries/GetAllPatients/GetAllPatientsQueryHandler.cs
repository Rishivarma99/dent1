using Dent1.Business.Abstractions;
using Dent1.Data;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Features.Patients.Queries.GetAllPatients;

public sealed class GetAllPatientsQueryHandler : IQueryHandler<GetAllPatientsQuery, List<PatientReadModel>>
{
    private readonly DentContext _dbContext;

    public GetAllPatientsQueryHandler(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PatientReadModel>> Handle(GetAllPatientsQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PatientReadModel(x.Id, x.Name, x.Phone, x.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
