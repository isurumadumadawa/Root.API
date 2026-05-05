using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Common.Abstractions;
using Root.API.Application.Common.Exceptions;
using Root.API.Application.Users.Commands;
using Root.API.Application.Users.Handlers;
using Root.API.Contracts.Responses.Users;
using Root.API.Domain.Constants;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Users.Handlers;

public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, UserDetailResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<UpdateMyProfileCommandHandler> _logger;

    public UpdateMyProfileCommandHandler(
        ApplicationDbContext db,
        ICurrentUserContext currentUser,
        ILogger<UpdateMyProfileCommandHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<UserDetailResponse> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Self profile update. UserId={UserId}", _currentUser.UserId);

        // FR-011/FR-029: Only user role may use this endpoint; username/role/createdDate are immutable
        if (_currentUser.Role != RoleSeeds.UserRoleName)
        {
            _logger.LogWarning(
                "Self-update rejected: caller role is not 'user'. Role={Role} UserId={UserId}",
                _currentUser.Role, _currentUser.UserId);
            throw new ForbiddenException("Only user-role accounts may use the self-update endpoint.");
        }

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User", _currentUser.UserId);

        // Only name and position are mutable (FR-011)
        user.Update(request.Name, request.Position);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Self profile updated. UserId={UserId}", user.Id);

        return CreateUserCommandHandler.MapToDetail(user, user.Role.Name);
    }
}
