namespace Dent1.Api.Contracts.Requests.Auth;

public sealed class LoginRequest
{
    public string UsernameOrPhone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
