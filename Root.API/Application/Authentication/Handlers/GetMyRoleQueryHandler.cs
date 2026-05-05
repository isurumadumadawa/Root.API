using MediatR;
using Microsoft.Extensions.Logging;
using Root.API.Application.Authentication.Queries;
using Root.API.Application.Common.Abstractions;
using Root.API.Application.Common.Exceptions;
using Root.API.Contracts.Responses.Auth;

namespace Root.API.Application.Authentication.Handlers;

public class GetMyRoleQueryHandler : IRequestHandler<GetMyRoleQuery, RoleResponse>
{
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<GetMyRoleQueryHandler> _logger;

    public GetMyRoleQueryHandler(ICurrentUserContext currentUser, ILogger<GetMyRoleQueryHandler> logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public Task<RoleResponse> Handle(GetMyRoleQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            _logger.LogWarning("Role resolution failed: user is not authenticated.");
            throw new UnauthorizedAccessException("Authentication required.");
        }

        _logger.LogInformation(
            "Role resolved for UserId={UserId}: {Role}",
            _currentUser.UserId, _currentUser.Role);

        return Task.FromResult(new RoleResponse { Role = _currentUser.Role });
    }
}
