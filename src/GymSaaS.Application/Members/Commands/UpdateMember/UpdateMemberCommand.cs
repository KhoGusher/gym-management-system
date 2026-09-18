using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Members.Commands.UpdateMember;

public record UpdateMemberCommand(
    Guid MemberId,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    DateTime? DateOfBirth
) : IRequest<bool>; // true = updated, false = not found