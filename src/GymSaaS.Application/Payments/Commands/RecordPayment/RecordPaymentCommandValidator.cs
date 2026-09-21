using FluentValidation;

namespace GymSaaS.Application.Payments.Commands.RecordPayment;

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x)
            .Must(x => x.DiscountAmount <= x.Amount)
            .WithMessage("Discount cannot exceed the payment amount.");
    }
}