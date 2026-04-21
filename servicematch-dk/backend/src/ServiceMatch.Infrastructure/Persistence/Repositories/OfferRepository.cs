using Microsoft.EntityFrameworkCore;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Infrastructure.Persistence.Repositories;

public sealed class OfferRepository(AppDbContext context) : IOfferRepository
{
    public Task<Offer?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.Offers.FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<Offer?> GetByIdWithNegotiationsAsync(Guid id, CancellationToken ct)
        => context.Offers
            .Include(o => o.Negotiations)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Offer>> GetByProviderIdAsync(Guid providerId, CancellationToken ct)
        => await context.Offers
            .Where(o => o.ServiceProviderId == providerId)
            .Include(o => o.Negotiations)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Offer offer, CancellationToken ct)
        => await context.Offers.AddAsync(offer, ct);

    public Task UpdateAsync(Offer offer, CancellationToken ct)
    {
        context.Offers.Update(offer);
        return Task.CompletedTask;
    }
}
