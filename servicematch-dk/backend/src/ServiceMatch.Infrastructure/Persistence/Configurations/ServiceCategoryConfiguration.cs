using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(100);
        builder.HasIndex(c => c.Slug).IsUnique();

        builder.HasData(
            new ServiceCategory(1, "Salon", "salon"),
            new ServiceCategory(2, "Negle", "nails"),
            new ServiceCategory(3, "Massage", "massage"),
            new ServiceCategory(4, "Tandlæge", "dentist"),
            new ServiceCategory(5, "Kiropraktor", "kiropraktor")
        );
    }
}
