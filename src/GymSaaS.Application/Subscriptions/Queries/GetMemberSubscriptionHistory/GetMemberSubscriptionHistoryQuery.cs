using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Subscriptions.Queries.GetMemberSubscriptionHistory;

public record SubscriptionHistoryItemDto(
    Guid Id, string PackageName, DateTime StartDateUtc, DateTime EndDateUtc, int Status, decimal AmountPaid);

public record GetMemberSubscriptionHistoryQuery(Guid MemberId) : IRequest<IReadOnlyList<SubscriptionHistoryItemDto>>;