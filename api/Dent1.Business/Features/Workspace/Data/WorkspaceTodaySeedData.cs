using Dent1.Business.Features.Workspace.Models;

namespace Dent1.Business.Features.Workspace.Data;

/// <summary>
/// Seed workspace payloads per clinician until appointment/visit tables exist.
/// </summary>
internal static class WorkspaceTodaySeedData
{
    private static readonly Guid KavyaIyerId = Guid.Parse("b1c2d3e4-1002-0000-0000-000000000002");

    private static readonly IReadOnlyList<WorkspaceQueueItemDto> DefaultQueue =
    [
        new(1, "A001", "Michael Thompson", "09:00 AM", "completed", "Root Canal"),
        new(2, "A002", "Sarah Martinez", "09:30 AM", "in-progress", "Cleaning"),
        new(3, "A003", "James Wilson", "10:00 AM", "arrived", "Consultation"),
        new(4, "A004", "Emily Davis", "10:30 AM", "scheduled", "Filling"),
        new(5, "A005", "Robert Brown", "11:00 AM", "scheduled", "Extraction")
    ];

    private static readonly IReadOnlyList<WorkspaceFollowUpDto> DefaultFollowUps =
    [
        new(1, "Patricia Miller", "Suggested", "2 days ago", "Root Canal Follow-up"),
        new(2, "Mark Anderson", "Pending", "5 days ago", "Extraction Check"),
        new(3, "Lisa Garcia", "Suggested", "1 week ago", "Crown Adjustment")
    ];

    private static readonly IReadOnlyList<WorkspaceQueueItemDto> AssistantQueue =
    [
        new(1, "A101", "David Lee", "09:00 AM", "completed", "Check-up"),
        new(2, "A102", "Anna White", "09:45 AM", "in-progress", "X-Ray Review"),
        new(3, "A103", "Chris Taylor", "10:15 AM", "arrived", "Prep for Extraction")
    ];

    private static readonly IReadOnlyList<WorkspaceFollowUpDto> AssistantFollowUps =
    [
        new(1, "Nina Patel", "Pending", "3 days ago", "Post-op Check"),
        new(2, "Tom Harris", "Suggested", "4 days ago", "Crown Fit")
    ];

    public static WorkspaceTodayResponse GetForUser(Guid userId)
    {
        if (userId == KavyaIyerId)
        {
            return Build(AssistantQueue, AssistantFollowUps);
        }

        // Default clinical payload (includes seeded doctor Arjun Rao and any unknown clinician).
        return Build(DefaultQueue, DefaultFollowUps);
    }

    private static WorkspaceTodayResponse Build(
        IReadOnlyList<WorkspaceQueueItemDto> queue,
        IReadOnlyList<WorkspaceFollowUpDto> followUps) =>
        new(queue, followUps, WorkspaceStatsBuilder.Build(queue, followUps.Count));
}
