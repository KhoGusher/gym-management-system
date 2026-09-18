using FluentValidation;

namespace GymSaaS.Application.Subscriptions.Commands.CreatePackage;

public class CreatePackageCommandValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.DurationInDays).GreaterThan(0).WithMessage("Duration must be at least 1 day.");
    }
}