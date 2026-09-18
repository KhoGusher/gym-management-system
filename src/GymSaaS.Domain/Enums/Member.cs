using GymSaaS.Domain.Common;
using GymSaaS.Domain.Enums;

namespace GymSaaS.Domain.Entities;

public class Member : BaseEntity, ITenantScoped
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; } // navigation back to the owning gym

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public MemberStatus Status { get; set; } = MemberStatus.Active;

    // Computed property — not stored in the DB, calculated on the fly.
    // This is a clean C# idiom you don't get for free in Java without a method call.
    public string FullName => $"{FirstName} {LastName}";
}