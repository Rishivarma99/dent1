using System.Security.Claims;
using Dent1.Api.Authorization;
using Dent1.Business.Abstractions;
using Dent1.Business.Features.Workspace.Models;
using Dent1.Business.Features.Workspace.Queries.GetWorkspaceToday;
using Dent1.Common.Authorization;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WorkspaceController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public WorkspaceController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    /// <summary>
    /// Today's clinical workspace for the authenticated doctor or assistant.
    /// </summary>
    [HasPermission(PermissionCodes.AppointmentRead)]
    [HttpGet("today")]
    public async Task<ActionResult<WorkspaceTodayResponse>> GetToday(CancellationToken cancellationToken)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role is not nameof(UserRole.Doctor) and not nameof(UserRole.Assistant))
        {
            throw new AppException(Errors.Auth.ScopeDenied);
        }

        var response = await _queryDispatcher.Dispatch(new GetWorkspaceTodayQuery(), cancellationToken);
        return Ok(response);
    }
}
