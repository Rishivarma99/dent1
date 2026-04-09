using Dent1.Business.Abstractions;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Features.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, CreatePatientResponse>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePatientResponse> Handle(CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var id = await _patientRepository.AddAsync(command.Name.Trim(), command.Phone.Trim(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _patientRepository.GetByIdAsync(id, cancellationToken);
        if (created is null)
        {
            throw new AppException(Errors.Patient.NotFound, new Dictionary<string, object> { ["PatientId"] = id });
        }

        return new CreatePatientResponse(created.Id, created.Name, created.Phone, created.CreatedAt);
    }
}
