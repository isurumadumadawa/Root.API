using MediatR;
using Root.API.Contracts.Responses.Auth;

namespace Root.API.Application.Authentication.Commands;

public record SignInCommand(string Username, string Password) : IRequest<SignInResponse>;
