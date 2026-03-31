using Dent1.Business.DTOs;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Queries;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// QUERY (Read Side of CQRS)
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// A Query represents a REQUEST FOR DATA — it never changes state.
// It returns a DTO, not the raw entity.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class GetPatientByIdQuery : IRequest<PatientDto?>
{
    public Guid Id { get; set; }

    public GetPatientByIdQuery(Guid id) => Id = id;
}

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// QUERY HANDLER
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// MediatR routes GetPatientByIdQuery → this handler.
// It ONLY reads data — no writes, no side effects.
// Returns a DTO (not the entity) — this is the read model.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientByIdQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id, cancellationToken);

        if (patient is null) return null;

        return new PatientDto
        {
            Id = patient.Id,
            Name = patient.Name,
            Phone = patient.Phone,
            CreatedAt = patient.CreatedAt
        };
    }
}
