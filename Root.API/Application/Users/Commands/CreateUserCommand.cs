using MediatR;
using Root.API.Contracts.Responses.Users;

namespace Root.API.Application.Users.Commands;

public record CreateUserCommand(
    string Name,
    string Username,
    string Password,
    string Role,
    string? Position
) : IRequest<UserDetailResponse>;
