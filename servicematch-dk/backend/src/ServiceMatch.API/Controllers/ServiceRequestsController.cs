using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMatch.Application.Features.ServiceRequests.Commands.CancelServiceRequest;
using ServiceMatch.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using ServiceMatch.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using ServiceMatch.Application.Features.ServiceRequests.Queries.GetServiceRequests;

namespace ServiceMatch.API.Controllers;

[ApiController]
[Route("api/v1/requests")]
[Authorize(Roles = "Client")]
public sealed class ServiceRequestsController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID not found."));

    [HttpGet]
    public async Task<IActionResult> GetMyRequests(CancellationToken ct)
        => Ok(await sender.Send(new GetServiceRequestsQuery(CurrentUserId), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetServiceRequestByIdQuery(id, CurrentUserId, User.FindFirst(ClaimTypes.Role)?.Value ?? "Client"), ct));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequestCommand command, CancellationToken ct)
    {
        var commandWithUser = command with { ClientId = CurrentUserId };
        var result = await sender.Send(commandWithUser, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        await sender.Send(new CancelServiceRequestCommand(id, CurrentUserId), ct);
        return NoContent();
    }
}
