using ServiceMatch.Domain.Exceptions;

namespace ServiceMatch.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money InDkk(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");
        return new Money(amount, "DKK");
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
