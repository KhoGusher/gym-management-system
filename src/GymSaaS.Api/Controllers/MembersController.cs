using GymSaaS.Application.Members.Commands.CreateMember;
using GymSaaS.Application.Members.Commands.DeactivateMember;
using GymSaaS.Application.Members.Commands.UpdateMember;
using GymSaaS.Application.Members.Queries.GetMemberById;
using GymSaaS.Application.Members.Queries.GetMembersList;
using GymSaaS.Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GymSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateMemberCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MemberDto>> GetById(Guid id, CancellationToken ct)
    {
        var member = await _mediator.Send(new GetMemberByIdQuery(id), ct);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<MemberListItemDto>>> GetList(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMembersListQuery(page, pageSize, search), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberCommand command, CancellationToken ct)
    {
        if (id != command.MemberId) return BadRequest("Route id and body MemberId must match.");
        var updated = await _mediator.Send(command, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeactivateMemberCommand(id), ct);
        return result ? NoContent() : NotFound();
    }
}