using MediatR;
using Root.API.Contracts.Responses.Users;

namespace Root.API.Application.Users.Queries;

public record GetUsersQuery : IRequest<IReadOnlyList<UserSummaryResponse>>;
