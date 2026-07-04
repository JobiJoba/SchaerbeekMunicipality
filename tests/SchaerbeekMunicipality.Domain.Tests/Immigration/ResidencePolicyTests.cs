using FluentAssertions;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;

namespace SchaerbeekMunicipality.Domain.Tests.Immigration;

public sealed class ResidencePolicyTests
{
    private static readonly ResidenceValidationContext EmptyDocuments = new(
        ResidenceCategory.EuCitizen,
        null,
        null,
        []);

    [Fact]
    public void EuCitizenPolicy_WithoutPermit_IsValid()
    {
        var policy = new EuCitizenPolicy();

        var result = policy.Validate(EmptyDocuments with { Category = ResidenceCategory.EuCitizen });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NonEuWorkerPolicy_WithoutPermit_IsInvalid()
    {
        var policy = new NonEuWorkerPolicy();

        var result = policy.Validate(EmptyDocuments with { Category = ResidenceCategory.NonEuWorker });

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("permit");
    }

    [Fact]
    public void NonEuWorkerPolicy_WithValidBCard_IsValid()
    {
        var policy = new NonEuWorkerPolicy();
        var permit = CreatePermit(ResidencePermitType.BCard, validUntil: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.NonEuWorker,
            Permit = permit,
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StudentPolicy_WithAnnex15_IsValid()
    {
        var policy = new StudentPolicy();
        var permit = CreatePermit(ResidencePermitType.Annex15, validUntil: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.Student,
            Permit = permit,
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EuCitizenPolicy_DoesNotRequireResidencePermitDocument()
    {
        var policy = new EuCitizenPolicy();

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.EuCitizen,
            AttachedDocumentTypes = [DocumentType.Passport],
        });

        result.IsValid.Should().BeTrue();
    }

    private static ResidencePermit CreatePermit(ResidencePermitType permitType, DateOnly validUntil)
    {
        return ResidencePermit.Create(
            Domain.Registration.RegistrationCaseId.New(),
            new ResidencePermitDetails(
                permitType,
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                validUntil,
                "BC-123",
                "Immigration Office"),
            DateTimeOffset.UtcNow);
    }
}
