namespace Root.API.Contracts.Requests.Users;

public class SelfUpdateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Position { get; init; }
}
