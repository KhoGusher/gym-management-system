using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Tenants.Queries.GetTenantDetail;

public class GetTenantDetailQueryHandler : IRequestHandler<GetTenantDetailQuery, TenantDetailDto?>
{
    private readonly IAppDbContext _dbContext;
    private readonly IIdentityService _identityService;

    public GetTenantDetailQueryHandler(IAppDbContext dbContext, IIdentityService identityService)
    {
        _dbContext = dbContext;
        _identityService = identityService;
    }

    public async Task<TenantDetailDto?> Handle(GetTenantDetailQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);
        if (tenant is null) return null;

        var users = await _identityService.GetUsersByTenantAsync(request.TenantId);

        // IgnoreQueryFilters is REQUIRED here: SuperAdmin's JWT carries no tenantId claim,
        // so the global filter (TenantId == currentTenantId) would evaluate to
        // "TenantId == null" and silently return zero rows. We bypass it and filter
        // explicitly by the requested tenant instead.
        var members = await _dbContext.Members
            .IgnoreQueryFilters()
            .Where(m => m.TenantId == request.TenantId && !m.IsDeleted)
            .OrderBy(m => m.LastName)
            .Select(m => new TenantMemberSummaryDto(m.Id, m.FullName, m.Email, m.Phone, (int)m.Status))
            .ToListAsync(cancellationToken);

        return new TenantDetailDto(tenant.Id, tenant.CompanyName, tenant.SubdomainSlug, tenant.ContactEmail, (int)tenant.Status, users, members);
    }
}