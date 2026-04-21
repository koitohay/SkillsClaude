using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FullName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.PasswordHash).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.Property(c => c.Email)
            .HasConversion(e => e.Value, v => EmailAddress.Create(v))
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(c => c.Email).IsUnique();

        builder.Property(c => c.PhoneNumber)
            .HasConversion(p => p.Value, v => DanishPhoneNumber.Create(v))
            .IsRequired()
            .HasMaxLength(20);
    }
}
