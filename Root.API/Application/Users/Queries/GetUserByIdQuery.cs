using MediatR;
using Root.API.Contracts.Responses.Users;

namespace Root.API.Application.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDetailResponse>;
