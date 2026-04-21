namespace ServiceMatch.Domain.Exceptions;

public sealed class InvalidCvrException(string input)
    : DomainException($"'{input}' is not a valid Danish CVR number. Must be exactly 8 digits.");
