using FluentAssertions;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegistrationCaseResidenceTests
{
    private static readonly OfficerId DemoOfficer = OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 4, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void SetResidenceCategory_WithoutIdentity_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        var act = () => registrationCase.SetResidenceCategory(ResidenceCategory.EuCitizen);

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void SetResidenceCategory_AfterIdentity_SetsCategory()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.RecordIdentity(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "French"));

        registrationCase.SetResidenceCategory(ResidenceCategory.EuCitizen);

        registrationCase.ResidenceCategory.Should().Be(ResidenceCategory.EuCitizen);
        registrationCase.Status.Should().Be(RegistrationCaseStatus.Intake);
    }

    [Fact]
    public void ApplyResidencePolicyResult_WhenValid_MarksLegalResidenceEstablished()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.ApplyResidencePolicyResult(ResidencePolicyResult.Valid());

        registrationCase.Checklist.LegalResidenceEstablished.Should().BeTrue();
    }
}
