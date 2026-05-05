using MediatR;
using Root.API.Contracts.Responses.Users;

namespace Root.API.Application.Users.Commands;

public record SoftDeleteUserCommand(Guid UserId) : IRequest<DeleteUserResponse>;
