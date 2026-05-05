using FluentValidation;
using Root.API.Application.Users.Commands;

namespace Root.API.Application.Users.Validators;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(120).WithMessage("Name must not exceed 120 characters.");

        RuleFor(x => x.Position)
            .MaximumLength(120).WithMessage("Position must not exceed 120 characters.")
            .When(x => x.Position is not null);
    }
}
