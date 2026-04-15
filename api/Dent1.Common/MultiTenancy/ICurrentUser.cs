namespace Dent1.Common.MultiTenancy;

/// <summary>
/// Represents the current user context.
/// Contains user identity and permissions.
/// </summary>
public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
    IReadOnlyCollection<string> Permissions { get; }
}
