using Dent1.Business.DTOs;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Queries;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// QUERY: Get all patients
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// No parameters needed — just "give me all patients".
// Returns List<PatientDto> — read-only DTOs.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class GetAllPatientsQuery : IRequest<List<PatientDto>>
{
}

public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, List<PatientDto>>
{
    private readonly IPatientRepository _patientRepository;

    public GetAllPatientsQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<List<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.GetAllAsync(cancellationToken);

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
