using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ServiceMatch.Application.Features.Auth.Commands.RegisterClient;
using ServiceMatch.Application.Features.Auth.Commands.RegisterProvider;
using ServiceMatch.Application.Features.Auth.Queries.Login;

namespace ServiceMatch.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[EnableRateLimiting("auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register/client")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterClientCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(RegisterClient), result);
    }

    [HttpPost("register/provider")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterProvider([FromBody] RegisterProviderCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(RegisterProvider), result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return Ok(result);
    }
}
