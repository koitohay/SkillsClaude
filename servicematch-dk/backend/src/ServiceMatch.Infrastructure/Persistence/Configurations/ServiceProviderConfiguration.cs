using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ContactName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.PasswordHash).IsRequired();

        builder.Property(p => p.Email)
            .HasConversion(e => e.Value, v => EmailAddress.Create(v))
            .IsRequired().HasMaxLength(256);
        builder.HasIndex(p => p.Email).IsUnique();

        builder.Property(p => p.PhoneNumber)
            .HasConversion(ph => ph.Value, v => DanishPhoneNumber.Create(v))
            .IsRequired().HasMaxLength(20);

        builder.Property(p => p.City)
            .HasConversion(c => c.Name, v => DanishCity.Create(v))
            .IsRequired().HasMaxLength(100);

        builder.Property(p => p.CvrNumber)
            .HasConversion(c => c.Value, v => CvrNumber.Create(v))
            .IsRequired().HasMaxLength(8);
        builder.HasIndex(p => p.CvrNumber).IsUnique();

        builder.HasMany(p => p.Categories)
            .WithOne()
            .HasForeignKey(pc => pc.ServiceProviderId);
    }
}
