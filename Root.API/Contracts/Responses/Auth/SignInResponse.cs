namespace Root.API.Contracts.Responses.Auth;

public class SignInResponse
{
    public string Token { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTime IssuedAtUtc { get; init; }
    public DateTime? ExpiresAtUtc { get; init; } // null by design — FR-030
    public string Role { get; init; } = string.Empty;
}
