using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Root.API.Application.Authentication.Commands;
using Root.API.Application.Authentication.Queries;
using Root.API.Contracts.Requests.Auth;
using Root.API.Contracts.Responses;
using Root.API.Contracts.Responses.Auth;

namespace Root.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Sign in with username and password to receive a bearer token.</summary>
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SignInResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var command = new SignInCommand(request.Username, request.Password);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Resolve the role of the currently authenticated caller from their bearer token.</summary>
    [HttpGet("role")]
    [Authorize]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyRole()
    {
        var result = await _mediator.Send(new GetMyRoleQuery());
        return Ok(result);
    }
}
