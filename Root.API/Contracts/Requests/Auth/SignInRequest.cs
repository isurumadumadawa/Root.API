namespace Root.API.Contracts.Requests.Auth;

public class SignInRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
