using System.Text.RegularExpressions;
using ServiceMatch.Domain.Exceptions;

namespace ServiceMatch.Domain.ValueObjects;

public sealed record EmailAddress
{
    private static readonly Regex Pattern = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);
        var normalised = input.Trim().ToLowerInvariant();
        if (!Pattern.IsMatch(normalised))
            throw new DomainException($"'{input}' is not a valid email address.");

        return new EmailAddress(normalised);
    }

    public override string ToString() => Value;
}
