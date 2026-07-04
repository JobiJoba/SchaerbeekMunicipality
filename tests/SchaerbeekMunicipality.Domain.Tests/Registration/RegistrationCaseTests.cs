using FluentAssertions;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public class RegistrationCaseTests
{
    private static readonly OfficerId DemoOfficer = OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 3, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Open_ReturnsIntakeStatusWithEmptyChecklist()
    {
        var registrationCase = RegistrationCase.Open(
            VisitReason.FirstRegistration,
            DemoOfficer,
            OpenedAt);

        registrationCase.Id.Value.Should().NotBe(Guid.Empty);
        registrationCase.Status.Should().Be(RegistrationCaseStatus.Intake);
        registrationCase.VisitReason.Should().Be(VisitReason.FirstRegistration);
        registrationCase.Checklist.IdentityEstablished.Should().BeFalse();
    }

    [Fact]
    public void RecordIdentity_DuringIntake_MarksIdentityEstablishedWithoutChangingStatus()
    {
        var registrationCase = RegistrationCase.Open(
            VisitReason.FirstRegistration,
            DemoOfficer,
            OpenedAt);

        var identity = new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "Belgian");
        var person = registrationCase.RecordIdentity(identity);

        person.GivenName.Should().Be("Marie");
        registrationCase.PersonId.Should().Be(person.Id);
        registrationCase.Checklist.IdentityEstablished.Should().BeTrue();
        registrationCase.Status.Should().Be(RegistrationCaseStatus.Intake);
    }

    [Fact]
    public void RecordIdentity_WhenAlreadyRecorded_Throws()
    {
        var registrationCase = RegistrationCase.Open(
            VisitReason.FirstRegistration,
            DemoOfficer,
            OpenedAt);

        registrationCase.RecordIdentity(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "Belgian"));

        var act = () => registrationCase.RecordIdentity(
            new IdentityDetails("Jean", "Martin", new DateOnly(1985, 1, 1), "French"));

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void EnsureCanAttachDocuments_WhenNotIntake_Throws()
    {
        var registrationCase = RegistrationCase.Open(
            VisitReason.FirstRegistration,
            DemoOfficer,
            OpenedAt);

        typeof(RegistrationCase)
            .GetProperty(nameof(RegistrationCase.Status))!
            .SetValue(registrationCase, RegistrationCaseStatus.Approved);

        var act = () => registrationCase.EnsureCanAttachDocuments();

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }
}
