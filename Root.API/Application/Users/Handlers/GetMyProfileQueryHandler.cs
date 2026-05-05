using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Common.Abstractions;
using Root.API.Application.Common.Exceptions;
using Root.API.Application.Users.Handlers;
using Root.API.Application.Users.Queries;
using Root.API.Contracts.Responses.Users;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Users.Handlers;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserDetailResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<GetMyProfileQueryHandler> _logger;

    public GetMyProfileQueryHandler(
        ApplicationDbContext db,
        ICurrentUserContext currentUser,
        ILogger<GetMyProfileQueryHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<UserDetailResponse> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Self profile read. UserId={UserId}", _currentUser.UserId);

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("User", _currentUser.UserId);

        return CreateUserCommandHandler.MapToDetail(user, user.Role.Name);
    }
}
