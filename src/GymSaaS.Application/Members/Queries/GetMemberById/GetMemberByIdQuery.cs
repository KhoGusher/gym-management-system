using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Members.Queries.GetMemberById;

public record MemberDto(
    Guid Id, string FirstName, string LastName, string FullName,
    string? Email, string? Phone, DateTime? DateOfBirth, int Status);

public record GetMemberByIdQuery(Guid MemberId) : IRequest<MemberDto?>;