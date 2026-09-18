using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Members.Commands.DeactivateMember;

public record DeactivateMemberCommand(Guid MemberId) : IRequest<bool>;