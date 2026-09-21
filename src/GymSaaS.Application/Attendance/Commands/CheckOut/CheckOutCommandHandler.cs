using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Attendance.Commands.CheckOut;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, bool>
{
    private readonly IAppDbContext _dbContext;

    public CheckOutCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<bool> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        var openRecord = await _dbContext.AttendanceRecords
            .Where(a => a.MemberId == request.MemberId && a.CheckOutUtc == null)
            .OrderByDescending(a => a.CheckInUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (openRecord is null) return false;

        openRecord.CheckOutUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}