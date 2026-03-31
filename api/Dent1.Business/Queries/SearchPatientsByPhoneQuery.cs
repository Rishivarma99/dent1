using Dent1.Business.DTOs;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Queries;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// QUERY: Search patients by phone number
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// From the PRD: "Phone is NOT unique. System suggests
// duplicates based on phone. Receptionist decides reuse or new."
// This query supports that duplicate detection flow.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class SearchPatientsByPhoneQuery : IRequest<List<PatientDto>>
{
    public string Phone { get; set; } = string.Empty;

    public SearchPatientsByPhoneQuery(string phone) => Phone = phone;
}

public class SearchPatientsByPhoneQueryHandler : IRequestHandler<SearchPatientsByPhoneQuery, List<PatientDto>>
{
    private readonly IPatientRepository _patientRepository;

    public SearchPatientsByPhoneQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<List<PatientDto>> Handle(SearchPatientsByPhoneQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.SearchByPhoneAsync(request.Phone, cancellationToken);

        return patients
            .Select(p => new PatientDto
            {
                Id = p.Id,
                Name = p.Name,
                Phone = p.Phone,
                CreatedAt = p.CreatedAt
            })
            .ToList();
    }
}
