using GymSaaS.Domain.Common;

namespace GymSaaS.Domain.Entities;

public class SubscriptionPackage : BaseEntity, ITenantScoped
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public required string Name { get; set; }            // "1 Month Unlimited"
    public string? Description { get; set; }
    public required decimal Price { get; set; }           // MWK
    public required int DurationInDays { get; set; }
    public bool IsActive { get; set; } = true;             // gym can retire a package without deleting history

    public ICollection<MemberSubscription> MemberSubscriptions { get; set; } = new List<MemberSubscription>();
}