using GymSaaS.Domain.Common;
using GymSaaS.Domain.Enums;

namespace GymSaaS.Domain.Entities;

public class MemberSubscription : BaseEntity, ITenantScoped
{
    public Guid TenantId { get; set; }

    public required Guid MemberId { get; set; }
    public Member? Member { get; set; }

    public required Guid SubscriptionPackageId { get; set; }
    public SubscriptionPackage? SubscriptionPackage { get; set; }

    public required DateTime StartDateUtc { get; set; }
    public required DateTime EndDateUtc { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public decimal AmountPaid { get; set; }

    // Computed — no stored column, always accurate
    public bool IsExpired => DateTime.UtcNow > EndDateUtc;
    public int DaysRemaining => IsExpired ? 0 : (EndDateUtc - DateTime.UtcNow).Days;
}