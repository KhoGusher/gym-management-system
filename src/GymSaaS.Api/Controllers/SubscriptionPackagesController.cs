using GymSaaS.Application.Common.Mediator;
using GymSaaS.Application.Subscriptions.Commands.CreatePackage;
using GymSaaS.Application.Subscriptions.Queries.GetPackages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionPackagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionPackagesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize(Roles = "GymOwner")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePackageCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetList), new { }, id);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PackageDto>>> GetList([FromQuery] bool activeOnly = true, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetPackagesQuery(activeOnly), ct));
}