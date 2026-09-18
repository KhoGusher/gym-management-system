using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Members.Commands.CreateMember;

public record CreateMemberCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DateTime? DateOfBirth
) : IRequest<Guid>;