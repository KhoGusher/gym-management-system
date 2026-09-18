using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Entities;
using GymSaaS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Subscriptions.Commands.SubscribeMember;

public class SubscribeMemberCommandHandler : IRequestHandler<SubscribeMemberCommand, Guid>
{
    private readonly IAppDbContext _dbContext;

    public SubscribeMemberCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<Guid> Handle(SubscribeMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken)
            ?? throw new KeyNotFoundException("Member not found.");

        var package = await _dbContext.SubscriptionPackages
            .FirstOrDefaultAsync(p => p.Id == request.PackageId && p.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException("Package not found or inactive.");

        // Business rule: no stacking an active subscription on top of another active one
        var hasActiveSubscription = await _dbContext.MemberSubscriptions
            .AnyAsync(ms => ms.MemberId == request.MemberId
                          && ms.Status == SubscriptionStatus.Active
                          && ms.EndDateUtc > DateTime.UtcNow, cancellationToken);

        if (hasActiveSubscription)
            throw new InvalidOperationException("Member already has an active subscription. Use renew instead.");

        var subscription = new MemberSubscription
        {
            MemberId = member.Id,
            SubscriptionPackageId = package.Id,
            StartDateUtc = DateTime.UtcNow,
            EndDateUtc = DateTime.UtcNow.AddDays(package.DurationInDays),
            AmountPaid = request.AmountPaid,
            Status = SubscriptionStatus.Active
        };

        _dbContext.MemberSubscriptions.Add(subscription);

        member.Status = MemberStatus.Active; // reactivate member status if they'd expired/lapsed

        await _dbContext.SaveChangesAsync(cancellationToken);

        return subscription.Id;
    }
}