using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Profile.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler(
    IClientRepository clientRepo,
    IServiceProviderRepository providerRepo)
    : IRequestHandler<GetMyProfileQuery, object>
{
    public async Task<object> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        if (request.Role == "Client")
        {
            var client = await clientRepo.GetByIdAsync(request.UserId, ct)
                ?? throw new NotFoundException("Client not found.");
            return new ClientDto(client.Id, client.FullName, client.Email.Value, client.PhoneNumber.Value);
        }

        var provider = await providerRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("Provider not found.");

        return new ServiceProviderDto(
            provider.Id,
            provider.CompanyName,
            provider.ContactName,
            provider.Email.Value,
            provider.PhoneNumber.Value,
            provider.Address,
            provider.City.Name,
            provider.CvrNumber.Value,
            provider.IsVerified,
            provider.Categories.Select(c => c.ServiceCategoryId).ToList().AsReadOnly(),
            provider.Services.Select(s => new ProviderServiceListingDto(s.Id, s.Name, s.Description, s.BasePrice.Amount, s.CategoryId)).ToList().AsReadOnly());
    }
}
