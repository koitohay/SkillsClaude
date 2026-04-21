using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Entities;

public class ProviderService
{
    public Guid Id { get; private set; }
    public Guid ServiceProviderId { get; private set; }
    public int? CategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money BasePrice { get; private set; } = null!;

    private ProviderService() { }

    public static ProviderService Create(
        Guid providerId,
        int? categoryId,
        string name,
        string description,
        decimal basePrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        return new ProviderService
        {
            Id = Guid.NewGuid(),
            ServiceProviderId = providerId,
            CategoryId = categoryId,
            Name = name.Trim(),
            Description = description.Trim(),
            BasePrice = Money.InDkk(basePrice),
        };
    }

    public void Update(string name, string description, decimal basePrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Name = name.Trim();
        Description = description.Trim();
        BasePrice = Money.InDkk(basePrice);
    }
}
