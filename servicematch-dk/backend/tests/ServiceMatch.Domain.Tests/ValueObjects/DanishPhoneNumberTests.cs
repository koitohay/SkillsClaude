using FluentAssertions;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Tests.ValueObjects;

public class DanishPhoneNumberTests
{
    [Theory]
    [InlineData("+4520123456")]
    [InlineData("004520123456")]
    [InlineData("20123456")]
    [InlineData("+45 20 12 34 56")]
    [InlineData("20-12-34-56")]
    public void Create_WithValidInput_NormalisesToPlusFormat(string input)
    {
        var phone = DanishPhoneNumber.Create(input);
        phone.Value.Should().StartWith("+45");
        phone.Value.Should().HaveLength(11);
    }

    [Theory]
    [InlineData("12345678")]   // starts with 1 — invalid
    [InlineData("+46201234")]  // wrong country code
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("2012345")]    // only 7 digits
    public void Create_WithInvalidInput_ThrowsInvalidPhoneNumberException(string input)
    {
        var act = () => DanishPhoneNumber.Create(input);
        act.Should().Throw<InvalidPhoneNumberException>();
    }

    [Fact]
    public void Create_WithNull_ThrowsArgumentException()
    {
        var act = () => DanishPhoneNumber.Create(null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TwoPhoneNumbers_WithSameNormalisedValue_AreEqual()
    {
        var a = DanishPhoneNumber.Create("+4520123456");
        var b = DanishPhoneNumber.Create("20123456");
        a.Should().Be(b);
    }
}
