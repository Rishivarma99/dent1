namespace Dent1.Api.Contracts.Responses.Auth;

public sealed class TokenPairResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
