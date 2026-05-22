using Dent1.Business.Abstractions;
using Dent1.Business.Features.Workspace.Data;
using Dent1.Business.Features.Workspace.Models;
using Dent1.Common.MultiTenancy;

namespace Dent1.Business.Features.Workspace.Queries.GetWorkspaceToday;

public sealed class GetWorkspaceTodayQueryHandler : IQueryHandler<GetWorkspaceTodayQuery, WorkspaceTodayResponse>
{
    private readonly ICurrentUser _currentUser;

    public GetWorkspaceTodayQueryHandler(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public Task<WorkspaceTodayResponse> Handle(GetWorkspaceTodayQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == Guid.Empty)
        {
            throw new InvalidOperationException("Authenticated user id is required for workspace today.");
        }

        return Task.FromResult(WorkspaceTodaySeedData.GetForUser(userId));
    }
}
