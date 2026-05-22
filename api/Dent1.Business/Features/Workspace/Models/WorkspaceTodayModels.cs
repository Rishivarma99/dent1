namespace Dent1.Business.Features.Workspace.Models;

public sealed record WorkspaceTodayResponse(
    IReadOnlyList<WorkspaceQueueItemDto> Queue,
    IReadOnlyList<WorkspaceFollowUpDto> FollowUps,
    WorkspaceStatsDto Stats);

public sealed record WorkspaceQueueItemDto(
    int Id,
    string AppointmentId,
    string Name,
    string Time,
    string Status,
    string Service);

public sealed record WorkspaceFollowUpDto(
    int Id,
    string Name,
    string Type,
    string Days,
    string Reason);

public sealed record WorkspaceStatsDto(IReadOnlyList<WorkspaceStatCardDto> Cards);

public sealed record WorkspaceStatCardDto(
    string Label,
    string Value,
    string Hint,
    string Icon,
    string Tone);
