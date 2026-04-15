namespace Dent1.Business.Abstractions;

/// <summary>
/// Lightweight auth state for token validation (minimal DB lookup).
/// Contains only the fields needed to validate that a token is still valid.
/// </summary>
public record TokenAuthState(
    Guid UserId,
    Guid TenantId,
    bool UserIsActive,
    bool TenantIsActive,
    string SecurityStamp);

/// <summary>
/// Reads the current auth state for a user from DB (lightweight query).
/// Used per-request to validate that a JWT token is still allowed.
/// </summary>
public interface ITokenAuthStateReader
{
    Task<TokenAuthState?> GetAuthStateAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken);
}
