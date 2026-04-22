namespace ServiceMatch.Application.DTOs;

public record FeaturedOfferDto(
    Guid OfferId,
    Guid ServiceRequestId,
    string CategoryName,
    string City,
    decimal Price,
    string? ProviderMessage,
    string CurationReason,
    DateTimeOffset OfferCreatedAt);
