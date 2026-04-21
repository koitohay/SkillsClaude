using System.Text.RegularExpressions;
using ServiceMatch.Domain.Exceptions;

namespace ServiceMatch.Domain.ValueObjects;

public sealed record CvrNumber
{
    private static readonly Regex Pattern = new(@"^[0-9]{8}$", RegexOptions.Compiled);

    public string Value { get; }

    private CvrNumber(string value) => Value = value;

    public static CvrNumber Create(string input)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(input)) throw new InvalidCvrException(input);
        var trimmed = input.Trim();
        if (!Pattern.IsMatch(trimmed) || trimmed == "00000000")
            throw new InvalidCvrException(input);

        return new CvrNumber(trimmed);
    }

    public override string ToString() => Value;
}
