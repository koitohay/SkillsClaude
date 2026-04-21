using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Offers.Commands.SubmitOffer;

public record SubmitOfferCommand(
    Guid ServiceRequestId,
    Guid ProviderId,
    decimal Price,
    string? Message) : IRequest<OfferDto>;
