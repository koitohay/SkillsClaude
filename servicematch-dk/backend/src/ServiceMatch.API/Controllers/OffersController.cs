using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMatch.Application.Features.Offers.Commands.AcceptOffer;
using ServiceMatch.Application.Features.Offers.Commands.CounterOffer;
using ServiceMatch.Application.Features.Offers.Commands.DeclineOffer;
using ServiceMatch.Application.Features.Offers.Commands.RespondToNegotiation;
using ServiceMatch.Application.Features.Offers.Commands.SubmitOffer;
using ServiceMatch.Application.Features.Offers.Queries.GetOffersForRequest;
using ServiceMatch.Domain.Enums;

namespace ServiceMatch.API.Controllers;

[ApiController]
[Route("api/v1/requests/{requestId:guid}/offers")]
[Authorize]
public sealed class OffersController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID not found."));

    [HttpGet]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetOffers(Guid requestId, CancellationToken ct)
        => Ok(await sender.Send(new GetOffersForRequestQuery(requestId), ct));

    [HttpPost]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> SubmitOffer(Guid requestId, [FromBody] SubmitOfferRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SubmitOfferCommand(requestId, CurrentUserId, request.Price, request.Message), ct);
        return CreatedAtAction(nameof(GetOffers), new { requestId }, result);
    }

    [HttpPut("{offerId:guid}/accept")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> AcceptOffer(Guid requestId, Guid offerId, CancellationToken ct)
    {
        await sender.Send(new AcceptOfferCommand(requestId, offerId, CurrentUserId), ct);
        return NoContent();
    }

    [HttpPut("{offerId:guid}/decline")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeclineOffer(Guid requestId, Guid offerId, CancellationToken ct)
    {
        await sender.Send(new DeclineOfferCommand(requestId, offerId, CurrentUserId), ct);
        return NoContent();
    }

    [HttpPost("{offerId:guid}/counter")]
    [Authorize]
    public async Task<IActionResult> Counter(Guid requestId, Guid offerId, [FromBody] CounterRequest request, CancellationToken ct)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        var initiatedBy = role == "Client" ? NegotiationInitiator.Client : NegotiationInitiator.Provider;
        var result = await sender.Send(
            new CounterOfferCommand(requestId, offerId, CurrentUserId, initiatedBy, request.ProposedPrice, request.Message, role), ct);
        return Ok(result);
    }

    [HttpPut("{offerId:guid}/negotiations/{negotiationId:guid}/accept")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> AcceptNegotiation(Guid requestId, Guid offerId, Guid negotiationId, CancellationToken ct)
    {
        await sender.Send(new RespondToNegotiationCommand(requestId, offerId, negotiationId, NegotiationResponse.Accept), ct);
        return NoContent();
    }

    [HttpPut("{offerId:guid}/negotiations/{negotiationId:guid}/decline")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> DeclineNegotiation(Guid requestId, Guid offerId, Guid negotiationId, CancellationToken ct)
    {
        await sender.Send(new RespondToNegotiationCommand(requestId, offerId, negotiationId, NegotiationResponse.Decline), ct);
        return NoContent();
    }

    [HttpPost("{offerId:guid}/negotiations/{negotiationId:guid}/counter")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> CounterNegotiation(Guid requestId, Guid offerId, [FromBody] CounterRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new CounterOfferCommand(requestId, offerId, CurrentUserId, NegotiationInitiator.Provider, request.ProposedPrice, request.Message, "Provider"), ct);
        return Ok(result);
    }
}

public record SubmitOfferRequest(decimal Price, string? Message);
public record CounterRequest(decimal ProposedPrice, string? Message);
