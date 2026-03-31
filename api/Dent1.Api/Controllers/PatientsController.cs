using Dent1.Business.Commands;
using Dent1.Business.DTOs;
using Dent1.Business.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// CQRS CONTROLLER
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// The controller has ZERO business logic.
// It only does two things:
//   1. Creates a Command or Query object
//   2. Sends it through MediatR
// MediatR routes it to the correct Handler automatically.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── COMMAND: Create Patient (Write Side) ─────────────────
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreatePatientCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    // ── QUERY: Get All Patients (Read Side) ──────────────────
    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetAll()
    {
        var patients = await _mediator.Send(new GetAllPatientsQuery());
        return Ok(patients);
    }

    // ── QUERY: Get Patient By Id (Read Side) ─────────────────
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id)
    {
        var patient = await _mediator.Send(new GetPatientByIdQuery(id));
        if (patient is null) return NotFound();
        return Ok(patient);
    }

    // ── QUERY: Search by Phone (Read Side) ───────────────────
    // Supports PRD's duplicate detection: "Phone is NOT unique.
    // System suggests duplicates based on phone."
    [HttpGet("search")]
    public async Task<ActionResult<List<PatientDto>>> SearchByPhone([FromQuery] string phone)
    {
        var patients = await _mediator.Send(new SearchPatientsByPhoneQuery(phone));
        return Ok(patients);
    }
}
