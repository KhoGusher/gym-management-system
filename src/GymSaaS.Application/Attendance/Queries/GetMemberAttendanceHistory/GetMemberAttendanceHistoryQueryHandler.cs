using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Attendance.Queries.GetMemberAttendanceHistory;

public class GetMemberAttendanceHistoryQueryHandler : IRequestHandler<GetMemberAttendanceHistoryQuery, IReadOnlyList<AttendanceHistoryItemDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetMemberAttendanceHistoryQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<AttendanceHistoryItemDto>> Handle(GetMemberAttendanceHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.AttendanceRecords
            .Where(a => a.MemberId == request.MemberId)
            .OrderByDescending(a => a.CheckInUtc)
            .Select(a => new AttendanceHistoryItemDto(a.Id, a.CheckInUtc, a.CheckOutUtc, (int)a.Method))
            .ToListAsync(cancellationToken);
    }
}