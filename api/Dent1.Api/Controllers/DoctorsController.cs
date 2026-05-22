using Dent1.Api.Authorization;
using Dent1.Api.Contracts.Requests.Doctors;
using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Commands.CreateDoctor;
using Dent1.Business.Features.Doctors.Commands.DeleteDoctor;
using Dent1.Business.Features.Doctors.Commands.UpdateDoctor;
using Dent1.Business.Features.Doctors.Models;
using Dent1.Business.Features.Doctors.Queries.GetAllDoctors;
using Dent1.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;

    public DoctorsController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
    }

    [HasPermission(PermissionCodes.UserRead)]
    [HttpGet]
    public async Task<ActionResult<List<DoctorReadModel>>> GetAll(CancellationToken cancellationToken)
    {
        var doctors = await _queryDispatcher.Dispatch(new GetAllDoctorsQuery(), cancellationToken);
        return Ok(doctors);
    }

    [HasPermission(PermissionCodes.UserManage)]
    [HttpPost]
    public async Task<ActionResult<DoctorReadModel>> Create(
        CreateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _commandDispatcher.Dispatch(
            new CreateDoctorCommand(request.Name, request.Specialty),
            cancellationToken);
        return CreatedAtAction(nameof(GetAll), created);
    }

    [HasPermission(PermissionCodes.UserManage)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DoctorReadModel>> Update(
        Guid id,
        UpdateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _commandDispatcher.Dispatch(
            new UpdateDoctorCommand(id, request.Name, request.Specialty),
            cancellationToken);

        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HasPermission(PermissionCodes.UserManage)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _commandDispatcher.Dispatch(new DeleteDoctorCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
