using Microsoft.EntityFrameworkCore;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Interfaces;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Repositories;

public sealed class ServiceProviderRepository(AppDbContext context) : IServiceProviderRepository
{
    public Task<ServiceProvider?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.ServiceProviders
            .Include(p => p.Categories)
            .Include(p => p.Services)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<ServiceProvider?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var emailVo = EmailAddress.Create(email);
        return context.ServiceProviders.FirstOrDefaultAsync(p => p.Email == emailVo, ct);
    }

    public Task<ServiceProvider?> GetByCvrAsync(string cvr, CancellationToken ct)
    {
        var cvrVo = CvrNumber.Create(cvr);
        return context.ServiceProviders.FirstOrDefaultAsync(p => p.CvrNumber == cvrVo, ct);
    }

    public async Task<IReadOnlyList<ServiceProvider>> GetByCategoryAsync(int? categoryId, CancellationToken ct)
    {
        var query = context.ServiceProviders
            .Include(p => p.Categories)
            .Include(p => p.Services)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.Categories.Any(c => c.ServiceCategoryId == categoryId.Value));

        return await query.OrderBy(p => p.CompanyName).ToListAsync(ct);
    }

    public async Task AddAsync(ServiceProvider provider, CancellationToken ct)
        => await context.ServiceProviders.AddAsync(provider, ct);

    public Task UpdateAsync(ServiceProvider provider, CancellationToken ct)
    {
        context.ServiceProviders.Update(provider);
        return Task.CompletedTask;
    }
}
