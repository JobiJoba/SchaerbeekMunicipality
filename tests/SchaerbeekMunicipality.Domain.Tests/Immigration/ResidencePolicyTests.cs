using FluentAssertions;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Immigration;

public sealed class ResidencePolicyTests
{
    private static readonly IReadOnlyList<DocumentType> PassportOnly = [DocumentType.Passport];

    private static readonly ResidenceValidationContext EmptyDocuments = new(
        ResidenceCategory.EuCitizen,
        null,
        null,
        []);

    [Fact]
    public void EuCitizenPolicy_WithoutIdentityDocument_IsInvalid()
    {
        var policy = new EuCitizenPolicy();

        var result = policy.Validate(EmptyDocuments with { Category = ResidenceCategory.EuCitizen });

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("passport");
    }

    [Fact]
    public void NonEuWorkerPolicy_WithoutPermit_IsInvalid()
    {
        var policy = new NonEuWorkerPolicy();

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.NonEuWorker,
            AttachedDocumentTypes = PassportOnly
        });

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("permit");
    }

    [Fact]
    public void NonEuWorkerPolicy_WithValidBCard_IsValid()
    {
        var policy = new NonEuWorkerPolicy();
        var permit = CreatePermit(ResidencePermitType.BCard, DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.NonEuWorker,
            Permit = permit,
            AttachedDocumentTypes = PassportOnly
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StudentPolicy_WithAnnex15_IsValid()
    {
        var policy = new StudentPolicy();
        var permit = CreatePermit(ResidencePermitType.Annex15, DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.Student,
            Permit = permit,
            AttachedDocumentTypes = PassportOnly
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
            AttachedDocumentTypes = PassportOnly
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DiplomatPolicy_WithoutIdentityDocument_IsInvalid()
    {
        var policy = new DiplomatPolicy();

        var result = policy.Validate(EmptyDocuments with { Category = ResidenceCategory.Diplomat });

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("diplomatic");
    }

    [Fact]
    public void DiplomatPolicy_WithDiplomaticCard_IsValid()
    {
        var policy = new DiplomatPolicy();

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.Diplomat,
            AttachedDocumentTypes = [DocumentType.DiplomaticCard]
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DiplomatPolicy_WithPassport_IsValid()
    {
        var policy = new DiplomatPolicy();

        var result = policy.Validate(EmptyDocuments with
        {
            Category = ResidenceCategory.Diplomat,
            AttachedDocumentTypes = PassportOnly
        });

        result.IsValid.Should().BeTrue();
    }

    private static ResidencePermit CreatePermit(ResidencePermitType permitType, DateOnly validUntil)
    {
        return ResidencePermit.Create(
            RegistrationCaseId.New(),
            new ResidencePermitDetails(
                permitType,
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                validUntil,
                "BC-123",
                "Immigration Office"),
            DateTimeOffset.UtcNow);
    }
}