using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Tenants.Queries.GetTenantDetail;

public record TenantMemberSummaryDto(Guid Id, string FullName, string? Email, string? Phone, int Status);

public record TenantDetailDto(
    Guid Id, string CompanyName, string SubdomainSlug, string? ContactEmail, int Status,
    IReadOnlyList<UserSummaryDto> Users, IReadOnlyList<TenantMemberSummaryDto> Members);

public record GetTenantDetailQuery(Guid TenantId) : IRequest<TenantDetailDto?>;