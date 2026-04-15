using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dent1.Business.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, IUnitOfWork unitOfWork, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> CreatePatientAsync(string name, string phone, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new AppException(Errors.Common.ValidationFailed, new Dictionary<string, object>
            {
                ["Details"] = "Patient name is required"
            });
        }

        var existingWithPhone = await _patientRepository.SearchByPhoneAsync(phone, cancellationToken);
        var exactExists = existingWithPhone.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (exactExists)
        {
            _logger.LogWarning("Duplicate patient create attempt for phone {Phone}", phone);
            throw new AppException(Errors.Patient.Duplicate);
        }

        var id = await _patientRepository.AddAsync(name, phone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Patient created with ID {Id}", id);
        return id;
    }
}
