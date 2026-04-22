using Microsoft.EntityFrameworkCore;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Infrastructure.Persistence.Repositories;

public sealed class ServiceRequestRepository(AppDbContext context) : IServiceRequestRepository
{
    public Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<ServiceRequest?> GetByIdWithOffersAsync(Guid id, CancellationToken ct)
        => context.ServiceRequests
            .Include(r => r.Category)
            .Include(r => r.Offers).ThenInclude(o => o.Negotiations)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<ServiceRequest>> GetByClientIdAsync(Guid clientId, CancellationToken ct)
        => await context.ServiceRequests
            .Include(r => r.Category)
            .Include(r => r.Offers).ThenInclude(o => o.Negotiations)
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ServiceRequest>> GetRelevantForProviderAsync(Guid providerId, CancellationToken ct)
    {
        var providerCategoryIds = await context.ProviderCategories
            .Where(pc => pc.ServiceProviderId == providerId)
            .Select(pc => pc.ServiceCategoryId)
            .ToListAsync(ct);

        return await context.ServiceRequests
            .Include(r => r.Category)
            .Include(r => r.Offers).ThenInclude(o => o.Negotiations)
            .Where(r => r.Status == ServiceRequestStatus.Open || r.Status == ServiceRequestStatus.OfferReceived)
            .Where(r => r.CategoryId == null || providerCategoryIds.Contains(r.CategoryId.Value))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ServiceRequest request, CancellationToken ct)
        => await context.ServiceRequests.AddAsync(request, ct);

    public Task UpdateAsync(ServiceRequest request, CancellationToken ct)
    {
        foreach (var offer in request.Offers)
        {
            if (context.Entry(offer).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                context.Offers.Add(offer);

            foreach (var negotiation in offer.Negotiations)
            {
                if (context.Entry(negotiation).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                    context.Negotiations.Add(negotiation);
            }
        }
        return Task.CompletedTask;
    }
}
