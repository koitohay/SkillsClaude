using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMatch.Application.Features.Providers.Queries.GetProvidersByCategory;
using ServiceMatch.Application.Features.Providers.Queries.GetRelevantRequests;
using ServiceMatch.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

namespace ServiceMatch.API.Controllers;

[ApiController]
[Route("api/v1/providers")]
public sealed class ProvidersController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID not found."));

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProviders([FromQuery] int? categoryId, [FromQuery] string? searchTerm, CancellationToken ct)
        => Ok(await sender.Send(new GetProvidersByCategoryQuery(categoryId, searchTerm), ct));

    [HttpGet("me/requests")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> GetRelevantRequests(CancellationToken ct)
        => Ok(await sender.Send(new GetRelevantRequestsQuery(CurrentUserId), ct));

    [HttpGet("me/requests/{id:guid}")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> GetRequestById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetServiceRequestByIdQuery(id, CurrentUserId, User.FindFirst(ClaimTypes.Role)?.Value ?? "Provider"), ct);
        if (result is not null)
        {
            var myOffer = result.Offers?.Where(o => o.ServiceProviderId == CurrentUserId).ToList();
            result = result with { Offers = myOffer };
        }
        return Ok(result);
    }
}
