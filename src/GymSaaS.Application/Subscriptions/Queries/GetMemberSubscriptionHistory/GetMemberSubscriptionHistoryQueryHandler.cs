using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Subscriptions.Queries.GetMemberSubscriptionHistory;

public class GetMemberSubscriptionHistoryQueryHandler : IRequestHandler<GetMemberSubscriptionHistoryQuery, IReadOnlyList<SubscriptionHistoryItemDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetMemberSubscriptionHistoryQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<SubscriptionHistoryItemDto>> Handle(GetMemberSubscriptionHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.MemberSubscriptions
            .Where(ms => ms.MemberId == request.MemberId)
            .OrderByDescending(ms => ms.StartDateUtc)
            .Select(ms => new SubscriptionHistoryItemDto(
                ms.Id, ms.SubscriptionPackage!.Name, ms.StartDateUtc, ms.EndDateUtc, (int)ms.Status, ms.AmountPaid))
            .ToListAsync(cancellationToken);
    }
}