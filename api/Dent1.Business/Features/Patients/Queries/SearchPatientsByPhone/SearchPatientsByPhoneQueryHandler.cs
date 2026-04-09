using Dent1.Business.Abstractions;
using Dent1.Business.Features.Patients.Queries.GetAllPatients;
using Dent1.Data;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Features.Patients.Queries.SearchPatientsByPhone;

public sealed class SearchPatientsByPhoneQueryHandler : IQueryHandler<SearchPatientsByPhoneQuery, List<PatientReadModel>>
{
    private readonly DentContext _dbContext;

    public SearchPatientsByPhoneQueryHandler(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PatientReadModel>> Handle(SearchPatientsByPhoneQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .Where(x => x.Phone == query.Phone)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PatientReadModel(x.Id, x.Name, x.Phone, x.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
