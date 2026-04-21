using FluentAssertions;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.ValueObjects;

namespace ServiceMatch.Domain.Tests.ValueObjects;

public class CvrNumberTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("00000001")]
    [InlineData("99999999")]
    public void Create_WithValidInput_ReturnsCvrNumber(string input)
    {
        var cvr = CvrNumber.Create(input);
        cvr.Value.Should().Be(input);
    }

    [Theory]
    [InlineData("1234567")]   // 7 digits
    [InlineData("123456789")] // 9 digits
    [InlineData("1234567a")]  // non-numeric
    [InlineData("00000000")]  // all zeros
    [InlineData("")]
    public void Create_WithInvalidInput_ThrowsInvalidCvrException(string input)
    {
        var act = () => CvrNumber.Create(input);
        act.Should().Throw<InvalidCvrException>();
    }

    [Fact]
    public void Create_WithNull_ThrowsArgumentException()
    {
        var act = () => CvrNumber.Create(null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TwoCvrNumbers_WithSameValue_AreEqual()
    {
        var a = CvrNumber.Create("12345678");
        var b = CvrNumber.Create("12345678");
        a.Should().Be(b);
    }
}
