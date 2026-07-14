using FluentAssertions;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegistrationCaseCorrectionTests
{
    private static readonly OfficerId DemoOfficer = OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 4, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CorrectIdentity_UpdatesLinkedPerson()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        var person = registrationCase.RecordIdentity(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "French"));

        registrationCase.CorrectIdentity(
            person,
            new IdentityDetails("Jean", "Dupont", new DateOnly(1990, 5, 15), "French"));

        person.GivenName.Should().Be("Jean");
        registrationCase.Checklist.IdentityEstablished.Should().BeTrue();
    }

    [Fact]
    public void CorrectIdentity_WhenIdentityNotRecorded_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        var person = Person.Create(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "French"));

        var act = () => registrationCase.CorrectIdentity(
            person,
            new IdentityDetails("Jean", "Dupont", new DateOnly(1990, 5, 15), "French"));

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void CorrectIdentity_WhenWrongPerson_Throws()
    {
        var registrationCase = OpenCaseWithIdentity();
        var otherPerson = Person.Create(
            new IdentityDetails("Other", "Person", new DateOnly(1980, 1, 1), "Belgian"));

        var act = () => registrationCase.CorrectIdentity(
            otherPerson,
            new IdentityDetails("Jean", "Dupont", new DateOnly(1990, 5, 15), "French"));

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void CorrectIdentity_WhenApproved_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        var person = registrationCase.RecordIdentity(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "French"));

        SetStatus(registrationCase, RegistrationCaseStatus.Approved);

        var act = () => registrationCase.CorrectIdentity(
            person,
            new IdentityDetails("Jean", "Dupont", new DateOnly(1990, 5, 15), "French"));

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void EnsureIntakeDataEditable_AllowsUnderReview()
    {
        var registrationCase = OpenCaseWithIdentity();
        SetStatus(registrationCase, RegistrationCaseStatus.UnderReview);

        var act = () => registrationCase.EnsureIntakeDataEditable("test");

        act.Should().NotThrow();
    }

    [Fact]
    public void SetResidenceCategory_AllowsUnderReview()
    {
        var registrationCase = OpenCaseWithIdentity();
        SetStatus(registrationCase, RegistrationCaseStatus.UnderReview);

        registrationCase.SetResidenceCategory(ResidenceCategory.EuCitizen);

        registrationCase.ResidenceCategory.Should().Be(ResidenceCategory.EuCitizen);
    }

    private static RegistrationCase OpenCaseWithIdentity()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.RecordIdentity(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "French"));

        return registrationCase;
    }

    private static void SetStatus(RegistrationCase registrationCase, RegistrationCaseStatus status)
    {
        typeof(RegistrationCase)
            .GetProperty(nameof(RegistrationCase.Status))!
            .SetValue(registrationCase, status);
    }
}