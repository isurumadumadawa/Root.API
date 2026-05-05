using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Common.Abstractions;
using Root.API.Application.Common.Exceptions;
using Root.API.Application.Users.Queries;
using Root.API.Contracts.Responses.Users;
using Root.API.Domain.Constants;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Users.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        ApplicationDbContext db,
        ICurrentUserContext currentUser,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<UserDetailResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching user. TargetUserId={UserId} CallerRole={Role}",
            request.UserId, _currentUser.Role);

        // FR-010/FR-012: user role can only access own profile
        if (_currentUser.Role == RoleSeeds.UserRoleName && _currentUser.UserId != request.UserId)
        {
            _logger.LogWarning(
                "Forbidden: user role attempted to access another user. CallerId={CallerId} TargetId={TargetId}",
                _currentUser.UserId, request.UserId);
            throw new ForbiddenException("You are not allowed to view other users' details.");
        }

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User", request.UserId);

        _logger.LogInformation("User fetched. UserId={UserId}", user.Id);

        return CreateUserCommandHandler.MapToDetail(user, user.Role.Name);
    }
}
