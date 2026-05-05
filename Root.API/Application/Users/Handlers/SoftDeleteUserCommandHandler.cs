using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Common.Exceptions;
using Root.API.Application.Users.Commands;
using Root.API.Contracts.Responses.Users;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Users.Handlers;

public class SoftDeleteUserCommandHandler : IRequestHandler<SoftDeleteUserCommand, DeleteUserResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SoftDeleteUserCommandHandler> _logger;

    public SoftDeleteUserCommandHandler(ApplicationDbContext db, ILogger<SoftDeleteUserCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<DeleteUserResponse> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Soft-deleting user. TargetUserId={UserId}", request.UserId);

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User", request.UserId);

        if (user.IsDeleted)
        {
            _logger.LogWarning("User already deleted. UserId={UserId}", user.Id);
            throw new DomainException("User is already deleted.", "already_deleted");
        }

        user.SoftDelete();
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User soft-deleted. UserId={UserId}", user.Id);

        return new DeleteUserResponse { UserId = user.Id, Status = "Deleted" };
    }
}
