using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto?>
{
    private readonly IAppDbContext _dbContext;

    public GetTenantByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TenantDto?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Tenants
            .Where(t => t.Id == request.TenantId)
            .Select(t => new TenantDto(t.Id, t.CompanyName, t.SubdomainSlug, t.ContactEmail, (int)t.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }
}   