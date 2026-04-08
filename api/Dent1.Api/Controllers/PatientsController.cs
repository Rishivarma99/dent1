using Dent1.Business.DTOs;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Entities;
using Dent1.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatientsController(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var id = await _patientRepository.AddAsync(request.Name, request.Phone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetAll(CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.GetAllAsync(cancellationToken);
        return Ok(patients.Select(MapToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            throw new AppException(Errors.Patient.NotFound, new Dictionary<string, object>
            {
                ["PatientId"] = id
            });
        }

        return Ok(MapToDto(patient));
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<PatientDto>>> SearchByPhone([FromQuery] string phone, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new AppException(Errors.Patient.InvalidSearchPhone);
        }

        var patients = await _patientRepository.SearchByPhoneAsync(phone, cancellationToken);
        return Ok(patients.Select(MapToDto).ToList());
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            Name = patient.Name,
            Phone = patient.Phone,
            CreatedAt = patient.CreatedAt
        };
    }

    public sealed class CreatePatientRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
