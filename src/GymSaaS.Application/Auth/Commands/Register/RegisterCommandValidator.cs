using FluentValidation;

namespace GymSaaS.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a number.");
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().Must(r => new[] { "GymOwner", "Staff", "Trainer" }.Contains(r))
            .WithMessage("Role must be GymOwner, Staff, or Trainer."); // SuperAdmin deliberately excluded from public registration
    }
}