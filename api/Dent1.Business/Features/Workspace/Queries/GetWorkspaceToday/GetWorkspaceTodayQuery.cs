using Dent1.Business.Abstractions;
using Dent1.Business.Features.Workspace.Models;

namespace Dent1.Business.Features.Workspace.Queries.GetWorkspaceToday;

public sealed record GetWorkspaceTodayQuery : IQuery<WorkspaceTodayResponse>;
