using Microsoft.EntityFrameworkCore;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Interfaces;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Repositories;

public sealed class ClientRepository(AppDbContext context) : IClientRepository
{
    public Task<Client?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.Clients.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Client?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var emailVo = EmailAddress.Create(email);
        return context.Clients.FirstOrDefaultAsync(c => c.Email == emailVo, ct);
    }

    public async Task AddAsync(Client client, CancellationToken ct)
        => await context.Clients.AddAsync(client, ct);

    public Task UpdateAsync(Client client, CancellationToken ct)
    {
        context.Clients.Update(client);
        return Task.CompletedTask;
    }
}
