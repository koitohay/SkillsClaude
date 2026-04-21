using MediatR;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Auth.Commands.RegisterProvider;

public sealed class RegisterProviderCommandHandler(
    IServiceProviderRepository providerRepo,
    IPasswordHasher passwordHasher,
    IUnitOfWork uow)
    : IRequestHandler<RegisterProviderCommand, ServiceProviderDto>
{
    public async Task<ServiceProviderDto> Handle(RegisterProviderCommand request, CancellationToken ct)
    {
        if (await providerRepo.GetByEmailAsync(request.Email, ct) is not null)
            throw new DomainException($"Email '{request.Email}' is already registered.");

        if (await providerRepo.GetByCvrAsync(request.CvrNumber, ct) is not null)
            throw new DomainException($"CVR '{request.CvrNumber}' is already registered.");

        var hash = passwordHasher.Hash(request.Password);
        var provider = Domain.Entities.ServiceProvider.Create(
            request.CompanyName, request.ContactName, request.Email,
            request.PhoneNumber, request.Address, request.City, request.CvrNumber, hash);

        provider.AssignCategories(request.CategoryIds);

        if (request.Services is not null)
            foreach (var s in request.Services)
                provider.AddService(s.CategoryId, s.Name, s.Description, s.BasePrice);

        await providerRepo.AddAsync(provider, ct);
        await uow.SaveChangesAsync(ct);

        return new ServiceProviderDto(
            provider.Id, provider.CompanyName, provider.ContactName,
            provider.Email.Value, provider.PhoneNumber.Value,
            provider.Address, provider.City.Name, provider.CvrNumber.Value,
            provider.IsVerified,
            provider.Categories.Select(c => c.ServiceCategoryId).ToList().AsReadOnly(),
            provider.Services.Select(s => new ProviderServiceListingDto(s.Id, s.Name, s.Description, s.BasePrice.Amount, s.CategoryId)).ToList().AsReadOnly());
    }
}
