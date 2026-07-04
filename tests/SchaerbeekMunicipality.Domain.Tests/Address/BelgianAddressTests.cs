using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;

namespace SchaerbeekMunicipality.Domain.Tests.Address;

public sealed class BelgianAddressTests
{
    [Fact]
    public void Create_WithValidSchaerbeekAddress_Succeeds()
    {
        var address = BelgianAddress.Create(
            "Chaussée de Louvain",
            "12",
            null,
            "1030",
            "Schaerbeek");

        address.PostalCode.Should().Be("1030");
        address.Municipality.Should().Be("Schaerbeek");
        address.FormatSingleLine().Should().Contain("Chaussée de Louvain");
    }

    [Theory]
    [InlineData("99")]
    [InlineData("12345")]
    [InlineData("abcd")]
    public void Create_WithInvalidPostalCode_Throws(string postalCode)
    {
        var act = () => BelgianAddress.Create(
            "Rue Test",
            "1",
            null,
            postalCode,
            "Schaerbeek");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithPostalCodeBelowRange_Throws()
    {
        var act = () => BelgianAddress.Create(
            "Rue Test",
            "1",
            null,
            "0999",
            "Test");

        act.Should().Throw<ArgumentException>();
    }
}
