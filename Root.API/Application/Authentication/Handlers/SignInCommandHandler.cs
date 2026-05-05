using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Root.API.Application.Abstractions;
using Root.API.Application.Authentication.Commands;
using Root.API.Application.Common.Exceptions;
using Root.API.Contracts.Responses.Auth;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Application.Authentication.Handlers;

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<SignInCommandHandler> _logger;

    public SignInCommandHandler(
        ApplicationDbContext db,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<SignInCommandHandler> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<SignInResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sign-in attempt for username: {Username}", request.Username);

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Sign-in failed: user not found. Username={Username}", request.Username);
            throw new DomainException("Invalid credentials.", "invalid_credentials");
        }

        // FR-027: Soft-deleted users must be blocked from sign-in
        if (user.IsDeleted)
        {
            _logger.LogWarning("Sign-in failed: user is deleted. UserId={UserId}", user.Id);
            throw new DomainException("Account is inactive.", "account_inactive");
        }

        var passwordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
        {
            _logger.LogWarning("Sign-in failed: invalid password. UserId={UserId}", user.Id);
            throw new DomainException("Invalid credentials.", "invalid_credentials");
        }

        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Role.Name);
        var issuedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "Sign-in successful. UserId={UserId} Role={Role}",
            user.Id, user.Role.Name);

        return new SignInResponse
        {
            Token = token,
            TokenType = "Bearer",
            IssuedAtUtc = issuedAt,
            ExpiresAtUtc = null, // FR-030: never expires
            Role = user.Role.Name
        };
    }
}
