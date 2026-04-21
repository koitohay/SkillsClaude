using ServiceMatch.Domain.Enums;

namespace ServiceMatch.Application.DTOs;

public record ServiceRequestDto(
    Guid Id,
    Guid ClientId,
    int? CategoryId,
    string? CategoryName,
    string? FreeTextDescription,
    DateOnly RequestedDate,
    TimeOnly RequestedTime,
    string City,
    ServiceRequestStatus Status,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OfferDto>? Offers = null);
