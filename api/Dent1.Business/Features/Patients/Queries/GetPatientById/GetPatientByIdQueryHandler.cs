using Dent1.Business.Abstractions;
using Dent1.Business.Features.Patients.Queries.GetAllPatients;
using Dent1.Data;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Features.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryHandler : IQueryHandler<GetPatientByIdQuery, PatientReadModel?>
{
    private readonly DentContext _dbContext;

    public GetPatientByIdQueryHandler(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PatientReadModel?> Handle(GetPatientByIdQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .Select(x => new PatientReadModel(x.Id, x.Name, x.Phone, x.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
