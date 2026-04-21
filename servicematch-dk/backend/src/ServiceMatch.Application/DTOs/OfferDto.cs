using ServiceMatch.Domain.Enums;

namespace ServiceMatch.Application.DTOs;

public record NegotiationDto(
    Guid Id,
    NegotiationInitiator InitiatedBy,
    decimal ProposedPrice,
    string? Message,
    NegotiationStatus Status,
    DateTimeOffset CreatedAt);

public record OfferDto(
    Guid Id,
    Guid ServiceRequestId,
    Guid ServiceProviderId,
    decimal Price,
    string? Message,
    OfferStatus Status,
    DateTimeOffset CreatedAt,
    IReadOnlyList<NegotiationDto> Negotiations);
