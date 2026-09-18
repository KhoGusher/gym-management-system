using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Tenants.Queries.GetTenantById;

public record TenantDto(Guid Id, string CompanyName, string SubdomainSlug, string? ContactEmail, int Status);

public record GetTenantByIdQuery(Guid TenantId) : IRequest<TenantDto?>;