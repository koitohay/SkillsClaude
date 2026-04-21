using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Domain.Interfaces;

public interface IServiceRequestRepository
{
    Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ServiceRequest?> GetByIdWithOffersAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceRequest>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceRequest>> GetRelevantForProviderAsync(Guid providerId, CancellationToken ct = default);
    Task AddAsync(ServiceRequest request, CancellationToken ct = default);
    Task UpdateAsync(ServiceRequest request, CancellationToken ct = default);
}
