using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Common.Exceptions;
using Root.API.Application.Users.Commands;
using Root.API.Contracts.Responses.Users;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Users.Handlers;

public class AdminUpdateUserCommandHandler : IRequestHandler<AdminUpdateUserCommand, UserDetailResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AdminUpdateUserCommandHandler> _logger;

    public AdminUpdateUserCommandHandler(ApplicationDbContext db, ILogger<AdminUpdateUserCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<UserDetailResponse> Handle(AdminUpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin updating user. TargetUserId={UserId}", request.UserId);

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User", request.UserId);

        Guid? newRoleId = null;
        string roleName = user.Role.Name;

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.Name == request.Role.ToLower(), cancellationToken);

            if (role is null)
                throw new DomainException($"Role '{request.Role}' does not exist.", "invalid_role");

            newRoleId = role.Id;
            roleName = role.Name;
        }

        user.Update(request.Name, request.Position, newRoleId);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User updated by admin. UserId={UserId}", user.Id);

        return CreateUserCommandHandler.MapToDetail(user, roleName);
    }
}
