using FluentValidation;
using Root.API.Application.Users.Commands;

namespace Root.API.Application.Users.Validators;

public class AdminUpdateUserCommandValidator : AbstractValidator<AdminUpdateUserCommand>
{
    private static readonly string[] AllowedRoles = ["user", "admin", "agent"];

    public AdminUpdateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(120).WithMessage("Name must not exceed 120 characters.");

        RuleFor(x => x.Position)
            .MaximumLength(120).WithMessage("Position must not exceed 120 characters.")
            .When(x => x.Position is not null);

        RuleFor(x => x.Role)
            .Must(r => r is null || AllowedRoles.Contains(r.ToLower()))
            .WithMessage("Role must be one of: user, admin, agent.")
            .When(x => x.Role is not null);
    }
}
