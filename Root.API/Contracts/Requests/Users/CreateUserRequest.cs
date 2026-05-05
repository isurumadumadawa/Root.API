namespace Root.API.Contracts.Requests.Users;

public class CreateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string? Position { get; init; }
}
