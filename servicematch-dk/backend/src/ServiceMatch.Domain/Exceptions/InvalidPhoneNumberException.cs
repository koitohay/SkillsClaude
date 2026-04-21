namespace ServiceMatch.Domain.Exceptions;

public sealed class InvalidPhoneNumberException(string input)
    : DomainException($"'{input}' is not a valid Danish phone number.");
