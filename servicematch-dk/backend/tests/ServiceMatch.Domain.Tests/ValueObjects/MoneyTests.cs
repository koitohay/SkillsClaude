using FluentAssertions;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(9999.99)]
    public void InDkk_WithNonNegativeAmount_CreatesMoney(decimal amount)
    {
        var money = Money.InDkk(amount);
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be("DKK");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void InDkk_WithNegativeAmount_ThrowsDomainException(decimal amount)
    {
        var act = () => Money.InDkk(amount);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TwoMoney_WithSameAmountAndCurrency_AreEqual()
    {
        Money.InDkk(100).Should().Be(Money.InDkk(100));
    }
}
