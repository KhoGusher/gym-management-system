using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Members.Queries.GetMembersList;

public class GetMembersListQueryHandler : IRequestHandler<GetMembersListQuery, PagedResult<MemberListItemDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetMembersListQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<MemberListItemDto>> Handle(GetMembersListQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Members.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(m =>
                m.FirstName.ToLower().Contains(term) ||
                m.LastName.ToLower().Contains(term) ||
                (m.Email != null && m.Email.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.LastName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MemberListItemDto(m.Id, m.FullName, m.Email, m.Phone, (int)m.Status))
            .ToListAsync(cancellationToken);

        return new PagedResult<MemberListItemDto>(items, totalCount, request.Page, request.PageSize);
    }
}