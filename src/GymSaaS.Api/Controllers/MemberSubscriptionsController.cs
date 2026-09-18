using GymSaaS.Application.Common.Mediator;
using GymSaaS.Application.Subscriptions.Commands.RenewSubscription;
using GymSaaS.Application.Subscriptions.Commands.SubscribeMember;
using GymSaaS.Application.Subscriptions.Queries.GetExpiringSoon;
using GymSaaS.Application.Subscriptions.Queries.GetMemberSubscriptionHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MemberSubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MemberSubscriptionsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("subscribe")]
    public async Task<ActionResult<Guid>> Subscribe([FromBody] SubscribeMemberCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("renew")]
    public async Task<ActionResult<Guid>> Renew([FromBody] RenewSubscriptionCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpGet("member/{memberId:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<SubscriptionHistoryItemDto>>> GetHistory(Guid memberId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetMemberSubscriptionHistoryQuery(memberId), ct));

    [HttpGet("expiring")]
    public async Task<ActionResult<IReadOnlyList<ExpiringSubscriptionDto>>> GetExpiring([FromQuery] int withinDays = 7, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetExpiringSoonQuery(withinDays), ct));
}