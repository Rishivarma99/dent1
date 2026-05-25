namespace Dent1.Api.Contracts.Requests.Auth;

public sealed class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
