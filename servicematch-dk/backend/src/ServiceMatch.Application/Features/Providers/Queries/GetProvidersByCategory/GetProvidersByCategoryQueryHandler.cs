using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Providers.Queries.GetProvidersByCategory;

public sealed class GetProvidersByCategoryQueryHandler(IServiceProviderRepository providerRepo)
    : IRequestHandler<GetProvidersByCategoryQuery, IReadOnlyList<ProviderWithServicesDto>>
{
    public async Task<IReadOnlyList<ProviderWithServicesDto>> Handle(
        GetProvidersByCategoryQuery request, CancellationToken ct)
    {
        var providers = await providerRepo.GetByCategoryAsync(request.CategoryId, ct);

        IEnumerable<Domain.Entities.ServiceProvider> filtered = providers;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLowerInvariant();
            filtered = filtered.Where(p =>
                p.CompanyName.ToLowerInvariant().Contains(term) ||
                p.Services.Any(s => s.Name.ToLowerInvariant().Contains(term)));
        }

        return filtered.Select(p => new ProviderWithServicesDto(
            p.Id,
            p.CompanyName,
            p.ContactName,
            p.City.Name,
            p.PhoneNumber.Value,
            p.Services
                .Where(s => request.CategoryId == null || s.CategoryId == request.CategoryId)
                .Select(s => new ProviderServiceListingDto(s.Id, s.Name, s.Description, s.BasePrice.Amount, s.CategoryId))
                .ToList()
        )).ToList();
    }
}
