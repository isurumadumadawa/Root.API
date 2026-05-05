using FluentValidation;
using Root.API.Application.Users.Commands;

namespace Root.API.Application.Users.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private static readonly string[] AllowedRoles = ["user", "admin", "agent"];

    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(120).WithMessage("Name must not exceed 120 characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(120).WithMessage("Username must not exceed 120 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => AllowedRoles.Contains(r?.ToLower()))
            .WithMessage("Role must be one of: user, admin, agent.");

        RuleFor(x => x.Position)
            .MaximumLength(120).WithMessage("Position must not exceed 120 characters.")
            .When(x => x.Position is not null);
    }
}
