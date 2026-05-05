namespace Root.API.Application.Common.Abstractions;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    string Username { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}
