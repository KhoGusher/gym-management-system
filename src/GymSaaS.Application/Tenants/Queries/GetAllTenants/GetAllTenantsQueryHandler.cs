using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using GymSaaS.Application.Tenants.Queries.GetTenantById;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Tenants.Queries.GetAllTenants;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, IReadOnlyList<TenantDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetAllTenantsQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Tenants
            .OrderBy(t => t.CompanyName)
            .Select(t => new TenantDto(t.Id, t.CompanyName, t.SubdomainSlug, t.ContactEmail, (int)t.Status))
            .ToListAsync(cancellationToken);
    }
}