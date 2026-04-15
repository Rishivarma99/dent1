namespace Dent1.Api.Contracts.Responses.Auth;

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public int AccessTokenExpiresInSeconds { get; set; }
}
