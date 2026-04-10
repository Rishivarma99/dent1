using Dent1.Business.Abstractions;
using Dent1.Business.Features.Patients.Commands.CreatePatient;
using Dent1.Business.Features.Patients.Queries.GetAllPatients;
using Dent1.Business.Features.Patients.Queries.GetPatientById;
using Dent1.Business.Features.Patients.Queries.SearchPatientsByPhone;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public PatientsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    /// <summary>
    /// Create a new patient.
    /// Requires: patient.create permission
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreatePatientResponse>> Create(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePatientCommand(request.Name, request.Phone);
        var response = await _commandDispatcher.Dispatch(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Get all patients.
    /// Requires: patient.read permission
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<PatientReadModel>>> GetAll(CancellationToken cancellationToken)
    {
        var patients = await _queryDispatcher.Dispatch(new GetAllPatientsQuery(), cancellationToken);
        return Ok(patients);
    }

    /// <summary>
    /// Search patients by phone number.
    /// Requires: patient.read permission
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<PatientReadModel>>> SearchByPhone([FromQuery] string phone, CancellationToken cancellationToken)
    {
        var patients = await _queryDispatcher.Dispatch(new SearchPatientsByPhoneQuery(phone), cancellationToken);
        return Ok(patients);
    }

    /// <summary>
    /// Get a specific patient by ID.
    /// Requires: patient.read permission and scope validation
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientReadModel>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await _queryDispatcher.Dispatch(new GetPatientByIdQuery(id), cancellationToken);
        if (patient is null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    public sealed record CreatePatientRequest(string Name, string Phone);
}
