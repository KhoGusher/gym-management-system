namespace GymSaaS.Application.Common.Interfaces;

// Application layer defines the contract. Infrastructure/Api will implement it
// (e.g. resolving tenant from subdomain, JWT claim, or header).
public interface ICurrentTenantService
{
    Guid? TenantId { get; }
}  