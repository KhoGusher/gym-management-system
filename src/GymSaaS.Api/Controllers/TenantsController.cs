using GymSaaS.Application.Tenants.Commands.CreateTenant;
using GymSaaS.Application.Tenants.Queries.GetAllTenants;
using GymSaaS.Application.Tenants.Queries.GetTenantById;
using GymSaaS.Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymSaaS.Application.Tenants.Queries.GetTenantDetail;

namespace GymSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTenantCommand command, CancellationToken ct)
    {
        var tenantId = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = tenantId }, tenantId);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TenantDto>>> GetList(CancellationToken ct)
    {
        var tenants = await _mediator.Send(new GetAllTenantsQuery(), ct);
        return Ok(tenants);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantDto>> GetById(Guid id, CancellationToken ct)
    {
        var tenant = await _mediator.Send(new GetTenantByIdQuery(id), ct);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpGet("{id:guid}/detail")]
    public async Task<ActionResult<TenantDetailDto>> GetDetail(Guid id, CancellationToken ct)
    {
        var detail = await _mediator.Send(new GetTenantDetailQuery(id), ct);
        return detail is null ? NotFound() : Ok(detail);
    }
}