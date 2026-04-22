using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.FreeTextDescription).HasMaxLength(2000);
        builder.Property(r => r.Status).IsRequired().HasConversion<string>();
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.Property(r => r.City)
            .HasConversion(c => c.Name, v => DanishCity.Create(v))
            .IsRequired().HasMaxLength(100);

        builder.HasMany(r => r.Offers)
            .WithOne()
            .HasForeignKey(o => o.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Offers).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Client>()
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
