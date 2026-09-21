using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Attendance.Queries.GetMemberAttendanceHistory;

public record AttendanceHistoryItemDto(Guid Id, DateTime CheckInUtc, DateTime? CheckOutUtc, int Method);

public record GetMemberAttendanceHistoryQuery(Guid MemberId) : IRequest<IReadOnlyList<AttendanceHistoryItemDto>>;