using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Enums;

namespace GymSaaS.Application.Payments.Commands.RecordPayment;

public record RecordPaymentCommand(
    Guid MemberId,
    Guid? MemberSubscriptionId,
    decimal Amount,
    decimal DiscountAmount,
    PaymentMethod Method,
    string? Notes
) : IRequest<PaymentReceiptDto>;

public record PaymentReceiptDto(Guid Id, string ReceiptNumber, decimal Amount, decimal DiscountAmount, decimal NetAmount, DateTime PaidAtUtc);