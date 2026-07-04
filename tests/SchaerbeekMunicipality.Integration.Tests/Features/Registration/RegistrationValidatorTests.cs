using FluentAssertions;
using FluentValidation.TestHelper;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class RegistrationValidatorTests
{
    private readonly OpenRegistrationCaseValidator _openValidator = new();
    private readonly RecordIdentityValidator _identityValidator = new();

    [Fact]
    public void OpenRegistrationCase_MissingVisitReason_FailsValidation()
    {
        var request = new OpenRegistrationCaseRequest((Domain.Registration.VisitReason)999, null);

        var result = _openValidator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.VisitReason);
    }

    [Fact]
    public void RecordIdentity_MissingGivenName_FailsValidation()
    {
        var request = new RecordIdentityRequest(
            string.Empty,
            "Dupont",
            new DateOnly(1990, 1, 1),
            "Belgian");

        var result = _identityValidator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [Fact]
    public void RecordIdentity_MissingNationality_FailsValidation()
    {
        var request = new RecordIdentityRequest(
            "Marie",
            "Dupont",
            new DateOnly(1990, 1, 1),
            string.Empty);

        var result = _identityValidator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Nationality);
    }
}
