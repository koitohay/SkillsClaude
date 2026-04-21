using ServiceMatch.Domain.Exceptions;

namespace ServiceMatch.Domain.ValueObjects;

public sealed record DanishCity
{
    public static readonly IReadOnlyList<string> KnownCities =
    [
        "København", "Aarhus", "Odense", "Aalborg", "Esbjerg",
        "Randers", "Kolding", "Vejle", "Horsens", "Helsingør",
        "Roskilde", "Silkeborg", "Næstved", "Fredericia", "Herning"
    ];

    public string Name { get; }

    private DanishCity(string name) => Name = name;

    public static DanishCity Create(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);
        var normalised = input.Trim();
        if (!KnownCities.Any(c => c.Equals(normalised, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"'{normalised}' is not a supported Danish city.");

        return new DanishCity(normalised);
    }

    public override string ToString() => Name;
}
