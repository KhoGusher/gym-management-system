using GymSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Member> Members { get; }

    DbSet<SubscriptionPackage> SubscriptionPackages { get; }
    DbSet<MemberSubscription> MemberSubscriptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DbSet<RefreshToken> RefreshTokens { get; }
}