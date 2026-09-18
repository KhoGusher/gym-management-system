using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Subscriptions.Commands.RenewSubscription;

public record RenewSubscriptionCommand(Guid MemberId, Guid PackageId, decimal AmountPaid) : IRequest<Guid>;