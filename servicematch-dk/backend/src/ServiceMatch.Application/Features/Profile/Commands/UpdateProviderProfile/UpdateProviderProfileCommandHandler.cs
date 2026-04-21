using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Profile.Commands.UpdateProviderProfile;

public sealed class UpdateProviderProfileCommandHandler(
    IServiceProviderRepository providerRepo,
    IUnitOfWork uow)
    : IRequestHandler<UpdateProviderProfileCommand, ServiceProviderDto>
{
    public async Task<ServiceProviderDto> Handle(UpdateProviderProfileCommand request, CancellationToken ct)
    {
        var provider = await providerRepo.GetByIdAsync(request.ProviderId, ct)
            ?? throw new NotFoundException("Provider not found.");

        provider.Update(request.CompanyName, request.ContactName, request.Email, request.PhoneNumber, request.Address, request.City);
        await providerRepo.UpdateAsync(provider, ct);
        await uow.SaveChangesAsync(ct);

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
