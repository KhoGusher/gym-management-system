namespace GymSaaS.Domain.Common;

// Any entity that belongs to a specific gym implements this.
// This is what lets us write ONE global EF Core query filter
// that automatically scopes every query to the current tenant —
// so a developer literally cannot forget a WHERE TenantId = ... clause.
public interface ITenantScoped
{
    Guid TenantId { get; set; }
}