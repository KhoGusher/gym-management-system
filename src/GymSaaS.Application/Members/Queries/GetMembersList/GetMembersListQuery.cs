using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Members.Queries.GetMembersList;

public record MemberListItemDto(Guid Id, string FullName, string? Email, string? Phone, int Status);

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public record GetMembersListQuery(int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<PagedResult<MemberListItemDto>>;