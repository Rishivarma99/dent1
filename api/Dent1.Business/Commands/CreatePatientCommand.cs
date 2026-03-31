using Dent1.Business.Behaviors;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Commands;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// COMMAND (Write Side of CQRS)
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// A Command represents an INTENT to change state.
// It carries the data needed for the operation.
// It does NOT return the full object — just the new Id.
//
// ITransactionalCommand → TransactionBehavior will wrap this
// in a DB transaction automatically via the MediatR pipeline.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class CreatePatientCommand : IRequest<Guid>, ITransactionalCommand
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// COMMAND HANDLER
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// The Handler is WHERE THE WORK HAPPENS.
// MediatR routes CreatePatientCommand → this handler automatically.
// This is the "service" in CQRS — but it only does ONE thing.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var id = await _patientRepository.AddAsync(request.Name, request.Phone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return id;
    }
}
