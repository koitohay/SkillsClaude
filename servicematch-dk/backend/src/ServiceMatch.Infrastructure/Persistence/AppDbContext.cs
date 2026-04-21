using Microsoft.EntityFrameworkCore;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ProviderCategory> ProviderCategories => Set<ProviderCategory>();
    public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Negotiation> Negotiations => Set<Negotiation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);
}
