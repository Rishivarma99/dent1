using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dent1.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dent1.Api.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    int GetAccessTokenLifetimeMinutes();
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

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