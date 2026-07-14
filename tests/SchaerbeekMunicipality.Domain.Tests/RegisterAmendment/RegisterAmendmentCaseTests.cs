using FluentAssertions;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.RegisterAmendment;

public sealed class RegisterAmendmentCaseTests
{
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 14, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Open_SetsDraftStatus()
    {
        var personId = PersonId.New();
        var amendmentCase = RegisterAmendmentCase.Open(
            personId,
            AmendmentType.IdentityCorrection,
            OpenedAt);

        amendmentCase.Status.Should().Be(RegisterAmendmentCaseStatus.Draft);
        amendmentCase.PersonId.Should().Be(personId);
        amendmentCase.AmendmentType.Should().Be(AmendmentType.IdentityCorrection);
    }

    [Fact]
    public void SubmitForReview_WithoutProposedChanges_Throws()
    {
        var amendmentCase = RegisterAmendmentCase.Open(
            PersonId.New(),
            AmendmentType.IdentityCorrection,
            OpenedAt);

        var act = () => amendmentCase.SubmitForReview(OpenedAt.AddHours(1));

        act.Should().Throw<InvalidRegisterAmendmentTransitionException>();
    }

    [Fact]
    public void SubmitForReview_WithoutDocument_Throws()
    {
        var amendmentCase = RegisterAmendmentCase.Open(
            PersonId.New(),
            AmendmentType.IdentityCorrection,
            OpenedAt);
        amendmentCase.RecordIdentityCorrection("Jean", "Martin");

        var act = () => amendmentCase.SubmitForReview(OpenedAt.AddHours(1));

        act.Should().Throw<InvalidRegisterAmendmentTransitionException>();
    }

    [Fact]
    public void Approve_FromDraft_Throws()
    {
        var amendmentCase = RegisterAmendmentCase.Open(
            PersonId.New(),
            AmendmentType.IdentityCorrection,
            OpenedAt);
        amendmentCase.RecordIdentityCorrection("Jean", "Martin");
        amendmentCase.MarkSupportingDocumentAttached();

        var act = () => amendmentCase.Approve(
            OfficerId.From(Guid.NewGuid()),
            null,
            OpenedAt.AddHours(2));

        act.Should().Throw<InvalidRegisterAmendmentTransitionException>();
    }

    [Fact]
    public void Apply_WithoutApproval_Throws()
    {
        var amendmentCase = CreateSubmittedCase();
        var person = CreatePerson();

        var act = () => amendmentCase.Apply(
            person,
            OfficerId.From(Guid.NewGuid()),
            OpenedAt.AddHours(3));

        act.Should().Throw<InvalidRegisterAmendmentTransitionException>();
    }

    [Fact]
    public void HappyPath_IdentityCorrection_ApplyUpdatesPersonName()
    {
        var person = CreatePerson();
        var amendmentCase = CreateSubmittedCaseForPerson(person.Id);
        var officer = OfficerId.From(Guid.NewGuid());

        amendmentCase.Approve(officer, "Court judgment verified.", OpenedAt.AddHours(2));

        var details = amendmentCase.Apply(person, officer, OpenedAt.AddHours(3));

        amendmentCase.Status.Should().Be(RegisterAmendmentCaseStatus.Applied);
        person.GivenName.Should().Be("Jean");
        person.FamilyName.Should().Be("Martin");
        details.ChangeSummary.Should().Contain("Martin");
    }

    [Fact]
    public void Apply_NationalityChange_UpdatesPersonNationality()
    {
        var person = CreatePerson();
        var amendmentCase = RegisterAmendmentCase.Open(
            person.Id,
            AmendmentType.NationalityChange,
            OpenedAt);
        amendmentCase.RecordNationalityChange("Belgian");
        amendmentCase.MarkSupportingDocumentAttached();
        amendmentCase.SubmitForReview(OpenedAt.AddHours(1));

        var officer = OfficerId.From(Guid.NewGuid());
        amendmentCase.Approve(officer, null, OpenedAt.AddHours(2));
        amendmentCase.Apply(person, officer, OpenedAt.AddHours(3));

        person.Nationality.Should().Be("Belgian");
    }

    [Fact]
    public void Apply_CivilStatusUpdate_UpdatesPersonCivilStatus()
    {
        var person = CreatePerson();
        var amendmentCase = RegisterAmendmentCase.Open(
            person.Id,
            AmendmentType.CivilStatusUpdate,
            OpenedAt);
        amendmentCase.RecordCivilStatusUpdate(new CivilStatusDetails(
            CivilStatus.Married,
            "Marie",
            "Dupont",
            new DateOnly(2020, 6, 15),
            "Schaerbeek",
            MarriageRecognitionStatus.Recognised));
        amendmentCase.MarkSupportingDocumentAttached();
        amendmentCase.SubmitForReview(OpenedAt.AddHours(1));

        var officer = OfficerId.From(Guid.NewGuid());
        amendmentCase.Approve(officer, null, OpenedAt.AddHours(2));
        amendmentCase.Apply(person, officer, OpenedAt.AddHours(3));

        person.CivilStatus.Should().NotBeNull();
        person.CivilStatus!.Status.Should().Be(CivilStatus.Married);
    }

    [Fact]
    public void RecordIdentityCorrection_WrongAmendmentType_Throws()
    {
        var amendmentCase = RegisterAmendmentCase.Open(
            PersonId.New(),
            AmendmentType.NationalityChange,
            OpenedAt);

        var act = () => amendmentCase.RecordIdentityCorrection("Jean", "Martin");

        act.Should().Throw<InvalidRegisterAmendmentTransitionException>();
    }

    private static RegisterAmendmentCase CreateSubmittedCaseForPerson(PersonId personId)
    {
        var amendmentCase = RegisterAmendmentCase.Open(
            personId,
            AmendmentType.IdentityCorrection,
            OpenedAt);
        amendmentCase.RecordIdentityCorrection("Jean", "Martin");
        amendmentCase.MarkSupportingDocumentAttached();
        amendmentCase.SubmitForReview(OpenedAt.AddHours(1));
        return amendmentCase;
    }

    private static RegisterAmendmentCase CreateSubmittedCase()
    {
        return CreateSubmittedCaseForPerson(PersonId.New());
    }

    private static Person CreatePerson()
    {
        return Person.Create(new IdentityDetails(
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            "Belgian"));
    }
}
