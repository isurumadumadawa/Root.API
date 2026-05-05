namespace Root.API.Contracts.Responses.Users;

public class DeleteUserResponse
{
    public Guid UserId { get; init; }
    public string Status { get; init; } = "Deleted";
}
