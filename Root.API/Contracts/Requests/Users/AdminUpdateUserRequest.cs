namespace Root.API.Contracts.Requests.Users;

public class AdminUpdateUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Position { get; init; }
    public string? Role { get; init; } // admin may update role
}
