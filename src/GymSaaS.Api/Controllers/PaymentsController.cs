using GymSaaS.Application.Common.Mediator;
using GymSaaS.Application.Payments.Commands.RecordPayment;
using GymSaaS.Application.Payments.Queries.GetMemberPaymentHistory;
using GymSaaS.Application.Payments.Queries.GetRevenueSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "GymOwner,Staff")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<PaymentReceiptDto>> Record([FromBody] RecordPaymentCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpGet("member/{memberId:guid}")]
    public async Task<ActionResult<IReadOnlyList<PaymentHistoryItemDto>>> GetHistory(Guid memberId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetMemberPaymentHistoryQuery(memberId), ct));

    [HttpGet("revenue-summary")]
    [Authorize(Roles = "GymOwner")]   // financial reporting restricted to the owner, not front-desk staff
    public async Task<ActionResult<RevenueSummaryDto>> GetRevenueSummary(
        [FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken ct)
        => Ok(await _mediator.Send(new GetRevenueSummaryQuery(fromUtc, toUtc), ct));
}