using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Members.Queries.GetMemberById;

public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto?>
{
    private readonly IAppDbContext _dbContext;

    public GetMemberByIdQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<MemberDto?> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Members
            .Where(m => m.Id == request.MemberId)
            .Select(m => new MemberDto(m.Id, m.FirstName, m.LastName, m.FullName, m.Email, m.Phone, m.DateOfBirth, (int)m.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }
}