namespace ServiceMatch.Application.DTOs;

public record ProviderServiceListingDto(
    Guid Id,
    string Name,
    string Description,
    decimal BasePrice,
    int? CategoryId);

public record ProviderWithServicesDto(
    Guid Id,
    string CompanyName,
    string ContactName,
    string City,
    string PhoneNumber,
    IReadOnlyList<ProviderServiceListingDto> Services);
