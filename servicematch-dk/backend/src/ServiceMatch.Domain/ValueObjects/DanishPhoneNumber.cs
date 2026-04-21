using System.Text.RegularExpressions;
using ServiceMatch.Domain.Exceptions;

namespace ServiceMatch.Domain.ValueObjects;

public sealed record DanishPhoneNumber
{
    // Accepts: +45XXXXXXXX, 0045XXXXXXXX, or 8-digit starting with 2-9
    private static readonly Regex Pattern = new(
        @"^(\+45|0045)?([2-9]\d{7})$",
        RegexOptions.Compiled);

    public string Value { get; }

    private DanishPhoneNumber(string value) => Value = value;

    public static DanishPhoneNumber Create(string input)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(input)) throw new InvalidPhoneNumberException(input);
        var trimmed = input.Replace(" ", "").Replace("-", "");
        var match = Pattern.Match(trimmed);
        if (!match.Success)
            throw new InvalidPhoneNumberException(input);

        var normalised = "+45" + match.Groups[2].Value;
        return new DanishPhoneNumber(normalised);
    }

    public override string ToString() => Value;
}
