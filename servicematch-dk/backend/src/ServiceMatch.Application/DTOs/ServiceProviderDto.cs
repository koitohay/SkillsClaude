namespace ServiceMatch.Application.DTOs;

public record ServiceProviderDto(
    Guid Id,
    string CompanyName,
    string ContactName,
    string Email,
    string PhoneNumber,
    string Address,
    string City,
    string CvrNumber,
    bool IsVerified,
    IReadOnlyList<int> CategoryIds,
    IReadOnlyList<ProviderServiceListingDto> Services);
