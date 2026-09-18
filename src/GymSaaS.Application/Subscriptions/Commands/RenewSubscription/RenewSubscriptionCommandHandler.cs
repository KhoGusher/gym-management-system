using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Entities;
using GymSaaS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Subscriptions.Commands.RenewSubscription;

public class RenewSubscriptionCommandHandler : IRequestHandler<RenewSubscriptionCommand, Guid>
{
    private readonly IAppDbContext _dbContext;

    public RenewSubscriptionCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<Guid> Handle(RenewSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var package = await _dbContext.SubscriptionPackages
            .FirstOrDefaultAsync(p => p.Id == request.PackageId && p.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException("Package not found or inactive.");

        // Find the member's most recent subscription (active or just-expired) to chain the new period from
        var currentSubscription = await _dbContext.MemberSubscriptions
            .Where(ms => ms.MemberId == request.MemberId)
            .OrderByDescending(ms => ms.EndDateUtc)
            .FirstOrDefaultAsync(cancellationToken);

        // Renewing before expiry extends from the current end date; renewing after expiry (or first time) starts fresh from now
        var startDate = currentSubscription is not null && currentSubscription.EndDateUtc > DateTime.UtcNow
            ? currentSubscription.EndDateUtc
            : DateTime.UtcNow;

        if (currentSubscription is not null && currentSubscription.Status == SubscriptionStatus.Active)
            currentSubscription.Status = SubscriptionStatus.Expired; // close out the old record cleanly

        var newSubscription = new MemberSubscription
        {
            MemberId = request.MemberId,
            SubscriptionPackageId = package.Id,
            StartDateUtc = startDate,
            EndDateUtc = startDate.AddDays(package.DurationInDays),
            AmountPaid = request.AmountPaid,
            Status = SubscriptionStatus.Active
        };

        _dbContext.MemberSubscriptions.Add(newSubscription);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return newSubscription.Id;
    }
}