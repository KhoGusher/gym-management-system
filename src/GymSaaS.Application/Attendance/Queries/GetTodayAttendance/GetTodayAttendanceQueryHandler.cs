using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Attendance.Queries.GetTodayAttendance;

public class GetTodayAttendanceQueryHandler : IRequestHandler<GetTodayAttendanceQuery, IReadOnlyList<TodayAttendanceItemDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetTodayAttendanceQueryHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<TodayAttendanceItemDto>> Handle(GetTodayAttendanceQuery request, CancellationToken cancellationToken)
    {
        var todayStartUtc = DateTime.UtcNow.Date;
        var todayEndUtc = todayStartUtc.AddDays(1);

        return await _dbContext.AttendanceRecords
            .Where(a => a.CheckInUtc >= todayStartUtc && a.CheckInUtc < todayEndUtc)
            .OrderByDescending(a => a.CheckInUtc)
            .Select(a => new TodayAttendanceItemDto(a.Id, a.MemberId, a.Member!.FullName, a.CheckInUtc, a.CheckOutUtc, (int)a.Method))
            .ToListAsync(cancellationToken);
    }
}