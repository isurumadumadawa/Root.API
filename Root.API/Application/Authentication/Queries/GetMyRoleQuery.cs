using MediatR;
using Root.API.Contracts.Responses.Auth;

namespace Root.API.Application.Authentication.Queries;

public record GetMyRoleQuery : IRequest<RoleResponse>;
