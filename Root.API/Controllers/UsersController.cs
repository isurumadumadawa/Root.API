using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Root.API.Application.Users.Commands;
using Root.API.Application.Users.Queries;
using Root.API.Contracts.Requests.Users;
using Root.API.Contracts.Responses;
using Root.API.Contracts.Responses.Users;

namespace Root.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── US2: Admin/Agent list all users ──────────────────────────────────────

    /// <summary>Get all users including soft-deleted (admin and agent only).</summary>
    [HttpGet]
    [Authorize(Policy = "AdminOrAgent")]
    [ProducesResponseType(typeof(IReadOnlyList<UserSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return Ok(result);
    }

    // ── US2: Admin create user ────────────────────────────────────────────────

    /// <summary>Create a new user (admin only).</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Name,
            request.Username,
            request.Password,
            request.Role,
            request.Position);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { userId = result.Id }, result);
    }

    // ── US2/US3: Get user by id ───────────────────────────────────────────────

    /// <summary>
    /// Get user detail by ID.
    /// Admin/agent may access any user. Standard user may only access their own record.
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(userId));
        return Ok(result);
    }

    // ── US2: Admin update any user ───────────────────────────────────────────

    /// <summary>Update user details (admin: any field; user role: use /users/me instead).</summary>
    [HttpPut("{userId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminUpdateUser(Guid userId, [FromBody] AdminUpdateUserRequest request)
    {
        var command = new AdminUpdateUserCommand(userId, request.Name, request.Position, request.Role);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // ── US2: Admin soft-delete ────────────────────────────────────────────────

    /// <summary>Soft-delete a user (admin only).</summary>
    [HttpPost("{userId:guid}/delete")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(DeleteUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _mediator.Send(new SoftDeleteUserCommand(userId));
        return Ok(result);
    }

    // ── US3: User self-service ────────────────────────────────────────────────

    /// <summary>Get own profile (authenticated user).</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _mediator.Send(new GetMyProfileQuery());
        return Ok(result);
    }

    /// <summary>Update own name and position (user role only — username/role/createdDate are immutable).</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] SelfUpdateUserRequest request)
    {
        var command = new UpdateMyProfileCommand(request.Name, request.Position);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
