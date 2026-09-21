using GymSaaS.Domain.Common;
using GymSaaS.Domain.Enums;

namespace GymSaaS.Domain.Entities;

public class Payment : BaseEntity, ITenantScoped
{
    public Guid TenantId { get; set; }

    public required Guid MemberId { get; set; }
    public Member? Member { get; set; }

    // Nullable — a payment isn't always tied to a subscription (e.g. a one-off service fee)
    public Guid? MemberSubscriptionId { get; set; }
    public MemberSubscription? MemberSubscription { get; set; }

    public required string ReceiptNumber { get; set; }   // human-facing, sequential per tenant — e.g. "IH-000042"
    public required decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public required PaymentMethod Method { get; set; }
    public string? Notes { get; set; }
    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;

    // Computed
    public decimal NetAmount => Amount - DiscountAmount;
}