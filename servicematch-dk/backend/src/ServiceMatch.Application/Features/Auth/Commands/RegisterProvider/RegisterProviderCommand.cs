using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Auth.Commands.RegisterProvider;

public record NewServiceDto(int? CategoryId, string Name, string Description, decimal BasePrice);

public record RegisterProviderCommand(
    string CompanyName,
    string ContactName,
    string Email,
    string PhoneNumber,
    string Address,
    string City,
    string CvrNumber,
    string Password,
    IReadOnlyList<int> CategoryIds,
    IReadOnlyList<NewServiceDto>? Services = null) : IRequest<ServiceProviderDto>;
