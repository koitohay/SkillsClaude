using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Domain.Interfaces;

public interface IOfferRepository
{
    Task<Offer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Offer?> GetByIdWithNegotiationsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Offer>> GetByProviderIdAsync(Guid providerId, CancellationToken ct = default);
    Task AddAsync(Offer offer, CancellationToken ct = default);
    Task UpdateAsync(Offer offer, CancellationToken ct = default);
}
