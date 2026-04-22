using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Message).HasMaxLength(2000);
        builder.Property(o => o.Status).IsRequired().HasConversion<string>();
        builder.Property(o => o.CreatedAt).IsRequired();

        builder.Property(o => o.Price)
            .HasConversion(m => m.Amount, v => Money.InDkk(v))
            .IsRequired()
            .HasColumnName("Price");

        builder.HasMany(o => o.Negotiations)
            .WithOne()
            .HasForeignKey(n => n.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Negotiations).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<ServiceProvider>()
            .WithMany()
            .HasForeignKey(o => o.ServiceProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
