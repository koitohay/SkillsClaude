using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class ProviderServiceConfiguration : IEntityTypeConfiguration<ProviderService>
{
    public void Configure(EntityTypeBuilder<ProviderService> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).IsRequired().HasMaxLength(1000);
        builder.Property(s => s.CategoryId).IsRequired(false);

        builder.Property(s => s.BasePrice)
            .HasConversion(m => m.Amount, v => Money.InDkk(v))
            .IsRequired()
            .HasColumnName("BasePrice");

        builder.HasOne<ServiceProvider>()
            .WithMany(p => p.Services)
            .HasForeignKey(s => s.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
