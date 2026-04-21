using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Enums;

namespace ServiceMatch.Application.Features.Offers.Commands.CounterOffer;

public record CounterOfferCommand(
    Guid ServiceRequestId,
    Guid OfferId,
    Guid InitiatorId,
    NegotiationInitiator InitiatedBy,
    decimal ProposedPrice,
    string? Message,
    string InitiatorRole) : IRequest<NegotiationDto>;
