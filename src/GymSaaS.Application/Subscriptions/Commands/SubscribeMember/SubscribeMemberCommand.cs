using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Subscriptions.Commands.SubscribeMember;

public record SubscribeMemberCommand(Guid MemberId, Guid PackageId, decimal AmountPaid) : IRequest<Guid>;