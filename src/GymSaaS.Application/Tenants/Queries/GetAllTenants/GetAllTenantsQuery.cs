using GymSaaS.Application.Common.Mediator;
using GymSaaS.Application.Tenants.Queries.GetTenantById;

namespace GymSaaS.Application.Tenants.Queries.GetAllTenants;

public record GetAllTenantsQuery : IRequest<IReadOnlyList<TenantDto>>;