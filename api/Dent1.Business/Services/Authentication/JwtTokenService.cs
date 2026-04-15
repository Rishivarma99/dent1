using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dent1.Business.Abstractions;
using Dent1.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dent1.Business.Security;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken);
    int GetAccessTokenLifetimeMinutes();
}

/// <summary>
/// Generates JWT access tokens with required claims per authentication rule.
/// 
/// Required claims:
/// - sub: user id
/// - tenant_id: tenant id
/// - security_stamp: current security stamp (for invalidation)
/// - permission: repeated claim for each permission
/// - role: user's role (optional but included)
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IPermissionResolver _permissionResolver;

    public JwtTokenService(IConfiguration configuration, IPermissionResolver permissionResolver)
    {
        _configuration = configuration;
        _permissionResolver = permissionResolver;
    }

    public async Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("tenant_id", user.TenantId.ToString()),
            new("security_stamp", user.SecurityStamp),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var permissions = await _permissionResolver.ResolveAsync(user.Id, user.TenantId, cancellationToken);
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(GetAccessTokenLifetimeMinutes());

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int GetAccessTokenLifetimeMinutes()
    {
        var configured = _configuration["Jwt:AccessTokenLifetimeMinutes"];
        return int.TryParse(configured, out var minutes) ? minutes : 30;
    }
}
