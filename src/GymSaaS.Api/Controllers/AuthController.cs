
using GymSaaS.Application.Auth.Commands.Login;
using GymSaaS.Application.Auth.Commands.Register;
using GymSaaS.Application.Auth.Commands.RefreshToken;
using GymSaaS.Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;


namespace GymSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));
}