using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Common.Exceptions;
using Root.API.Application.Users.Commands;
using Root.API.Application.Abstractions;
using Root.API.Contracts.Responses.Users;
using Root.API.Domain.Entities;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Users.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDetailResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        ApplicationDbContext db,
        IPasswordHasher passwordHasher,
        ILogger<CreateUserCommandHandler> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<UserDetailResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user. Username={Username} Role={Role}", request.Username, request.Role);

        var usernameExists = await _db.Users
            .AnyAsync(u => u.Username == request.Username, cancellationToken);

        if (usernameExists)
        {
            _logger.LogWarning("Create user failed: username conflict. Username={Username}", request.Username);
            throw new ConflictException($"Username '{request.Username}' is already taken.", "username_conflict");
        }

        var role = await _db.Roles
            .FirstOrDefaultAsync(r => r.Name == request.Role.ToLower(), cancellationToken);

        if (role is null)
            throw new DomainException($"Role '{request.Role}' does not exist.", "invalid_role");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(request.Name, request.Username, passwordHash, role.Id, request.Position);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User created. UserId={UserId} Role={Role}", user.Id, role.Name);

        return MapToDetail(user, role.Name);
    }

    internal static UserDetailResponse MapToDetail(User user, string roleName) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Username = user.Username,
        Position = user.Position,
        Role = roleName,
        CreatedDate = user.CreatedAtUtc,
        Status = user.Status.ToString()
    };
}
