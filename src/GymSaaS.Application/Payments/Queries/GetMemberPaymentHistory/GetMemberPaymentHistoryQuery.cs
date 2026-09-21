using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Payments.Queries.GetMemberPaymentHistory;

public record PaymentHistoryItemDto(
    Guid Id, string ReceiptNumber, decimal Amount, decimal DiscountAmount, decimal NetAmount,
    int Method, string? Notes, DateTime PaidAtUtc);

public record GetMemberPaymentHistoryQuery(Guid MemberId) : IRequest<IReadOnlyList<PaymentHistoryItemDto>>;