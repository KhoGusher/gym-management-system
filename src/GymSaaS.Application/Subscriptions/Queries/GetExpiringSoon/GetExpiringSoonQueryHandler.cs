using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Subscriptions.Queries.GetExpiringSoon;

public class GetExpiringSoonQueryHandler : IRequestHandler<GetExpiringSoonQuery, IReadOnlyList<ExpiringSubscriptionDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetExpiringSoonQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<ExpiringSubscriptionDto>> Handle(GetExpiringSoonQuery request, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(request.WithinDays);

        return await _dbContext.MemberSubscriptions
            .Where(ms => ms.Status == SubscriptionStatus.Active && ms.EndDateUtc <= cutoff && ms.EndDateUtc > DateTime.UtcNow)
            .OrderBy(ms => ms.EndDateUtc)
            .Select(ms => new ExpiringSubscriptionDto(
                ms.Id, ms.MemberId, ms.Member!.FullName, ms.Member.Phone,
                ms.SubscriptionPackage!.Name, ms.EndDateUtc, (ms.EndDateUtc - DateTime.UtcNow).Days))
            .ToListAsync(cancellationToken);
    }
}