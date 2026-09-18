using FluentValidation;

namespace GymSaaS.Application.Subscriptions.Commands.SubscribeMember;

public class SubscribeMemberCommandValidator : AbstractValidator<SubscribeMemberCommand>
{
    public SubscribeMemberCommandValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
        RuleFor(x => x.AmountPaid).GreaterThanOrEqualTo(0);
    }
}