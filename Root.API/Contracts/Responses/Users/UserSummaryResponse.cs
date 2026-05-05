namespace Root.API.Contracts.Responses.Users;

public class UserSummaryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string? Position { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public string Status { get; init; } = string.Empty;
}
