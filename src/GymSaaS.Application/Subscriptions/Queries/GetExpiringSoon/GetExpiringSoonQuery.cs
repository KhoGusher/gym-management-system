using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Subscriptions.Queries.GetExpiringSoon;

public record ExpiringSubscriptionDto(
    Guid SubscriptionId, Guid MemberId, string MemberFullName, string? MemberPhone,
    string PackageName, DateTime EndDateUtc, int DaysRemaining);

public record GetExpiringSoonQuery(int WithinDays = 7) : IRequest<IReadOnlyList<ExpiringSubscriptionDto>>;