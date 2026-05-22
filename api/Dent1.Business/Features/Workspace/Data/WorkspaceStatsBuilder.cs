using Dent1.Business.Features.Workspace.Models;

namespace Dent1.Business.Features.Workspace.Data;

internal static class WorkspaceStatsBuilder
{
    public static WorkspaceStatsDto Build(IReadOnlyList<WorkspaceQueueItemDto> queue, int followUpCount)
    {
        var completed = queue.Count(x => x.Status == "completed");
        var inQueue = queue.Count(x => x.Status != "completed");
        var inProgress = queue.Count(x => x.Status == "in-progress");

        return new WorkspaceStatsDto(
        [
            new("Today's Appointments", queue.Count.ToString(), $"{completed} completed", "pi-calendar", "primary"),
            new("In Queue", inQueue.ToString(), $"{inProgress} in progress", "pi-clock", "warning"),
            new("Completed Today", completed.ToString(), "Your visits", "pi-check-circle", "info"),
            new("Pending Follow-ups", followUpCount.ToString(), "Needs attention", "pi-exclamation-circle", "danger")
        ]);
    }
}
