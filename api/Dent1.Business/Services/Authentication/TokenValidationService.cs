using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Dent1.Business.Abstractions;

namespace Dent1.Business.Services;

/// <summary>
/// Validates JWT tokens per the authentication rule.
/// 
/// Validates in order:
/// 1. JWT signature is valid (done by middleware)
/// 2. Claims are present and parseable
/// 3. Current auth state from DB matches claims
/// </summary>
public sealed class TokenValidationService : ITokenValidationService
{
    private readonly ITokenAuthStateReader _authStateReader;

    public TokenValidationService(ITokenAuthStateReader authStateReader)
    {
        _authStateReader = authStateReader;
    }

    public async Task<TokenValidationResult> ValidateAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        // Step 1: Extract required claims
        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var tenantIdValue = principal.FindFirstValue("tenant_id");
        var securityStampValue = principal.FindFirstValue("security_stamp");

        // Step 2: Validate claim format
        if (!Guid.TryParse(userIdValue, out var userId))
            return new(false, "INVALID_USER_ID_CLAIM");

        if (!Guid.TryParse(tenantIdValue, out var tenantId))
            return new(false, "INVALID_TENANT_ID_CLAIM");

        if (string.IsNullOrWhiteSpace(securityStampValue))
            return new(false, "INVALID_SECURITY_STAMP_CLAIM");

        // Step 3: Validate current auth state (single lightweight query)
        var authState = await _authStateReader.GetAuthStateAsync(userId, tenantId, cancellationToken);

        if (authState is null)
            return new(false, "USER_OR_TENANT_NOT_FOUND");

        if (!authState.TenantIsActive)
            return new(false, "TENANT_INACTIVE");

        if (!authState.UserIsActive)
            return new(false, "USER_INACTIVE");

        // Step 4: Validate security stamp (most important: prevents old tokens from working)
        if (!string.Equals(authState.SecurityStamp, securityStampValue, StringComparison.Ordinal))
            return new(false, "SECURITY_STAMP_MISMATCH");

        return new(true);
    }
}
