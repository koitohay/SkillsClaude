using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Infrastructure.Persistence.Configurations;

public class NegotiationConfiguration : IEntityTypeConfiguration<Negotiation>
{
    public void Configure(EntityTypeBuilder<Negotiation> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Message).HasMaxLength(2000);
        builder.Property(n => n.Status).IsRequired().HasConversion<string>();
        builder.Property(n => n.InitiatedBy).IsRequired().HasConversion<string>();
        builder.Property(n => n.CreatedAt).IsRequired();

        builder.Property(n => n.ProposedPrice)
            .HasConversion(m => m.Amount, v => Money.InDkk(v))
            .IsRequired()
            .HasColumnName("ProposedPrice");
    }
}
