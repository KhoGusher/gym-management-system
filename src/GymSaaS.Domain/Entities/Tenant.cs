using GymSaaS.Domain.Common;
using GymSaaS.Domain.Enums;

namespace GymSaaS.Domain.Entities;

// A Tenant = one gym company that Gusherlabs has onboarded.
// Note: Tenant itself is NOT ITenantScoped — it IS the tenant boundary, not scoped by one.
public class Tenant : BaseEntity
{
    public required string CompanyName { get; set; }
    public required string SubdomainSlug { get; set; } // e.g. "ironhouse" -> ironhouse.gusherlabs.app
    public TenantStatus Status { get; set; } = TenantStatus.PendingSetup;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    // Navigation property — EF Core will use this to load related members
    public ICollection<Member> Members { get; set; } = new List<Member>();
}