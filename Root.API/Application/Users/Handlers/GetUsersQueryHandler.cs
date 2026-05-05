using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Common.Abstractions;
using Root.API.Application.Users.Queries;
using Root.API.Contracts.Responses.Users;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Users.Handlers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserSummaryResponse>>
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        ApplicationDbContext db,
        ICurrentUserContext currentUser,
        ILogger<GetUsersQueryHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserSummaryResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching all users. CallerRole={Role} CallerUserId={UserId}",
            _currentUser.Role, _currentUser.UserId);

        // Admin and agent see all users including soft-deleted (FR-007, FR-013, FR-028)
        var users = await _db.Users
            .Include(u => u.Role)
            .OrderBy(u => u.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Fetched {Count} users for CallerRole={Role}.",
            users.Count, _currentUser.Role);

        return users.Select(u => new UserSummaryResponse
        {
            Id = u.Id,
            Name = u.Name,
            Username = u.Username,
            Position = u.Position,
            Role = u.Role.Name,
            CreatedDate = u.CreatedAtUtc,
            Status = u.Status.ToString()
        }).ToList();
    }
}
