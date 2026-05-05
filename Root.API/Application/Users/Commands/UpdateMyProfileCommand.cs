using MediatR;
using Root.API.Contracts.Responses.Users;

namespace Root.API.Application.Users.Commands;

public record UpdateMyProfileCommand(string Name, string? Position) : IRequest<UserDetailResponse>;
