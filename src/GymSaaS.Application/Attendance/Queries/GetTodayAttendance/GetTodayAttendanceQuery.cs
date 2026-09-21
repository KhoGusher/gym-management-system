using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Attendance.Queries.GetTodayAttendance;

public record TodayAttendanceItemDto(
    Guid AttendanceId, Guid MemberId, string MemberFullName, DateTime CheckInUtc, DateTime? CheckOutUtc, int Method);

public record GetTodayAttendanceQuery : IRequest<IReadOnlyList<TodayAttendanceItemDto>>;