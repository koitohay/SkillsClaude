using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class ProviderCategoryConfiguration : IEntityTypeConfiguration<ProviderCategory>
{
    public void Configure(EntityTypeBuilder<ProviderCategory> builder)
    {
        builder.HasKey(pc => new { pc.ServiceProviderId, pc.ServiceCategoryId });

        builder.HasOne<ServiceCategory>()
            .WithMany()
            .HasForeignKey(pc => pc.ServiceCategoryId);
    }
}
