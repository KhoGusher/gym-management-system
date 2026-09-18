using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Subscriptions.Queries.GetPackages;

public class GetPackagesQueryHandler : IRequestHandler<GetPackagesQuery, IReadOnlyList<PackageDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetPackagesQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<PackageDto>> Handle(GetPackagesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.SubscriptionPackages.AsQueryable();

        if (request.ActiveOnly)
            query = query.Where(p => p.IsActive);

        return await query
            .OrderBy(p => p.Price)
            .Select(p => new PackageDto(p.Id, p.Name, p.Description, p.Price, p.DurationInDays, p.IsActive))
            .ToListAsync(cancellationToken);
    }
}