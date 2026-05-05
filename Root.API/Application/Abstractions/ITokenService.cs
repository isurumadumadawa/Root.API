namespace Root.API.Application.Abstractions;

public interface ITokenService
{
    string GenerateToken(Guid userId, string username, string role);
}
