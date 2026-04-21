using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Client?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Client client, CancellationToken ct = default);
    Task UpdateAsync(Client client, CancellationToken ct = default);
}
