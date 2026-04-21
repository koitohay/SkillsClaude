using Microsoft.EntityFrameworkCore;
using ServiceMatch.Application.Features.ServiceCategories.Queries.GetCategories;
using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Infrastructure.Persistence.Repositories;

public sealed class ServiceCategoryRepository(AppDbContext context) : IServiceCategoryRepository
{
    public async Task<IReadOnlyList<ServiceCategory>> GetAllAsync(CancellationToken ct)
        => await context.ServiceCategories.OrderBy(c => c.Id).ToListAsync(ct);
}
