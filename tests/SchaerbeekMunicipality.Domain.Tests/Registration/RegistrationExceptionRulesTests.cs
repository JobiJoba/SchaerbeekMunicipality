using FluentAssertions;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegistrationExceptionRulesTests
{
    [Fact]
    public void IsBirthEvidenceComplete_WhenPlaceAndCertificate_ReturnsTrue()
    {
        var person = Person.Create(new IdentityDetails("A", "B", new DateOnly(1990, 1, 1), "Belgian"));
        person.RecordBirthInformation(new BirthInformationDetails("Brussels", "Belgium"));

        RegistrationExceptionRules.IsBirthEvidenceComplete(
                person,
                [DocumentType.BirthCertificate])
            .Should().BeTrue();
    }

    [Fact]
    public void EuCitizenPolicy_WithoutIdentityDocument_IsInvalid()
    {
        var policy = new EuCitizenPolicy();
        var result = policy.Validate(new ResidenceValidationContext(
            ResidenceCategory.EuCitizen,
            null,
            null,
            []));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void EuCitizenPolicy_WithPassport_IsValid()
    {
        var policy = new EuCitizenPolicy();
        var result = policy.Validate(new ResidenceValidationContext(
            ResidenceCategory.EuCitizen,
            null,
            null,
            [DocumentType.Passport]));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RefugeePolicy_WithoutFederalDecision_IsInvalid()
    {
        var policy = new RefugeePolicy();
        var result = policy.Validate(new ResidenceValidationContext(
            ResidenceCategory.Refugee,
            null,
            null,
            []));

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("federal");
    }

    [Fact]
    public void RegisterTargetResolver_RefugeeWithDecision_SuggestsWaitingRegister()
    {
        RegisterTargetResolver.Suggest(ResidenceCategory.Refugee, "Syrian", true)
            .Should().Be(RegisterTarget.WaitingRegister);
    }

    [Fact]
    public void IsIllegalStay_ExcludesDiplomat()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        registrationCase.RecordIdentity(
            new IdentityDetails("Amara", "Diallo", new DateOnly(1985, 2, 1), "Senegalese"));
        registrationCase.SetResidenceCategory(ResidenceCategory.Diplomat);

        var invalid = ResidencePolicyResult.Invalid("A passport or diplomatic card must be attached.");

        RegistrationExceptionRules.IsIllegalStay(registrationCase, invalid).Should().BeFalse();
    }

    [Fact]
    public void CivilStatusRecord_ForeignMarriage_DefaultsToPendingRecognition()
    {
        var record = CivilStatusRecord.Create(new CivilStatusDetails(
            CivilStatus.Married,
            "Ali",
            "Benali",
            new DateOnly(2018, 3, 1),
            "Casablanca, Morocco"));

        record.MarriageRecognitionStatus.Should().Be(MarriageRecognitionStatus.PendingRecognition);
        record.EffectiveRegisterStatus.Should().Be(CivilStatus.Single);
    }
}