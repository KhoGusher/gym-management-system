using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Domain.Enums;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Members.Commands.DeactivateMember;

public class DeactivateMemberCommandHandler : IRequestHandler<DeactivateMemberCommand, bool>
{
    private readonly IAppDbContext _dbContext;

    public DeactivateMemberCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<bool> Handle(DeactivateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken);

        if (member is null) return false;

        member.Status = MemberStatus.Cancelled;
        member.IsDeleted = true; // vanishes from all normal queries via the global soft-delete filter, but the row stays in Postgres forever

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}