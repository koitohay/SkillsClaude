using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Domain.Interfaces;

public interface IServiceProviderRepository
{
    Task<ServiceProvider?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ServiceProvider?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<ServiceProvider?> GetByCvrAsync(string cvr, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceProvider>> GetByCategoryAsync(int? categoryId, CancellationToken ct = default);
    Task AddAsync(ServiceProvider provider, CancellationToken ct = default);
    Task UpdateAsync(ServiceProvider provider, CancellationToken ct = default);
}
