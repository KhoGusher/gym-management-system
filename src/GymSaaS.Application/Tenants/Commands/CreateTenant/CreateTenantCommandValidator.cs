using FluentValidation;

namespace GymSaaS.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200);

        RuleFor(x => x.SubdomainSlug)
            .NotEmpty()
            .Matches("^[a-z0-9-]+$").WithMessage("Slug must be lowercase letters, numbers, and hyphens only.")
            .MaximumLength(63); // real subdomain length limit

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Contact email must be valid.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Contact phone must be a valid international phone number starting with country code.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone));
    }
}