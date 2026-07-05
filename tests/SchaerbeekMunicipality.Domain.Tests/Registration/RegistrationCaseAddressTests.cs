using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegistrationCaseAddressTests
{
    private static readonly OfficerId DemoOfficer = OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 4, 10, 0, 0, TimeSpan.Zero);

    private static readonly AddressDetails SampleAddress = new(
        "Chaussée de Louvain",
        "42",
        null,
        "1030",
        "Schaerbeek");

    [Fact]
    public void DeclareAddress_WithoutIdentity_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        var act = () => registrationCase.DeclareAddress(SampleAddress);

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void DeclareAddress_AfterIdentity_MarksAddressDeclared()
    {
        var registrationCase = OpenCaseWithIdentity();

        registrationCase.DeclareAddress(SampleAddress);

        registrationCase.Checklist.AddressDeclared.Should().BeTrue();
        registrationCase.DeclaredAddress.Should().NotBeNull();
        registrationCase.DeclaredAddress!.PostalCode.Should().Be("1030");
    }

    [Fact]
    public void RecordHousingSituation_WithoutAddress_Throws()
    {
        var registrationCase = OpenCaseWithIdentity();

        var act = () => registrationCase.RecordHousingSituation(HousingSituation.Tenant);

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void RecordHousingSituation_AfterAddress_SetsSituation()
    {
        var registrationCase = OpenCaseWithIdentity();
        registrationCase.DeclareAddress(SampleAddress);

        registrationCase.RecordHousingSituation(HousingSituation.Tenant);

        registrationCase.HousingSituation.Should().Be(HousingSituation.Tenant);
    }

    [Fact]
    public void DeclareAddress_OutsideSchaerbeek_Throws()
    {
        var registrationCase = OpenCaseWithIdentity();

        var act = () => registrationCase.DeclareAddress(
            new AddressDetails("Rue de la Loi", "16", null, "1000", "Bruxelles"));

        act.Should().Throw<InvalidRegistrationTransitionException>()
            .WithMessage("*1030*Schaerbeek*");
    }

    private static RegistrationCase OpenCaseWithIdentity()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.RecordIdentity(
            new IdentityDetails("Jean", "Martin", new DateOnly(1985, 1, 10), "Belgian"));

        return registrationCase;
    }
}
