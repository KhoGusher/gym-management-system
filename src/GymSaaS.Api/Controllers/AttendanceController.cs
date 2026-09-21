using GymSaaS.Application.Attendance.Commands.CheckIn;
using GymSaaS.Application.Attendance.Commands.CheckOut;
using GymSaaS.Application.Attendance.Queries.GetMemberAttendanceHistory;
using GymSaaS.Application.Attendance.Queries.GetTodayAttendance;
using GymSaaS.Application.Common.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GymOwner,Staff")]
public class AttendanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendanceController(IMediator mediator) => _mediator = mediator;

    [HttpPost("check-in")]
    public async Task<ActionResult<CheckInResultDto>> CheckIn([FromBody] CheckInCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result ? NoContent() : NotFound("No open check-in found for this member.");
    }

    [HttpGet("today")]
    public async Task<ActionResult<IReadOnlyList<TodayAttendanceItemDto>>> GetToday(CancellationToken ct)
        => Ok(await _mediator.Send(new GetTodayAttendanceQuery(), ct));

    [HttpGet("member/{memberId:guid}")]
    public async Task<ActionResult<IReadOnlyList<AttendanceHistoryItemDto>>> GetHistory(Guid memberId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetMemberAttendanceHistoryQuery(memberId), ct));
}