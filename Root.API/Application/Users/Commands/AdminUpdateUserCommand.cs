using MediatR;
using Root.API.Contracts.Responses.Users;

namespace Root.API.Application.Users.Commands;

public record AdminUpdateUserCommand(
    Guid UserId,
    string Name,
    string? Position,
    string? Role
) : IRequest<UserDetailResponse>;
