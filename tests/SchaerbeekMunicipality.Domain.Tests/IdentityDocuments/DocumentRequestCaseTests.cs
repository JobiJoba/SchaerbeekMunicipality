using FluentAssertions;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.IdentityDocuments;

public sealed class DocumentRequestCaseTests
{
    private static readonly DateTimeOffset RequestedAt = new(2026, 7, 12, 10, 0, 0, TimeSpan.Zero);
    private static readonly PersonId PersonId = PersonId.New();
    private static readonly OfficerId OfficerId = OfficerId.From(Guid.NewGuid());

    [Fact]
    public void Open_SetsSubmittedStatus()
    {
        var documentRequestCase = DocumentRequestCase.Open(
            PersonId,
            DocumentRequestType.IdentityCardRenewal,
            RequestedAt);

        documentRequestCase.Status.Should().Be(DocumentRequestCaseStatus.Submitted);
        documentRequestCase.PersonId.Should().Be(PersonId);
        documentRequestCase.PhotoAttached.Should().BeFalse();
    }

    [Fact]
    public void AdvanceStatus_MovesForwardOnly()
    {
        var documentRequestCase = DocumentRequestCase.Open(
            PersonId,
            DocumentRequestType.PassportRenewal,
            RequestedAt);

        documentRequestCase.AttachApplicantPhoto(AdministrativeDocumentId.New());
        documentRequestCase.RecordFeePayment("STUB-FEE");

        documentRequestCase.AdvanceStatus();
        documentRequestCase.Status.Should().Be(DocumentRequestCaseStatus.InProduction);

        documentRequestCase.AdvanceStatus();
        documentRequestCase.Status.Should().Be(DocumentRequestCaseStatus.ReadyForCollection);
    }

    [Fact]
    public void AdvanceStatus_WithoutPhoto_Throws()
    {
        var documentRequestCase = DocumentRequestCase.Open(
            PersonId,
            DocumentRequestType.PassportRenewal,
            RequestedAt);

        documentRequestCase.RecordFeePayment("STUB-FEE");

        var act = () => documentRequestCase.AdvanceStatus();

        act.Should().Throw<InvalidDocumentRequestTransitionException>()
            .WithMessage("*photo*");
    }

    [Fact]
    public void AdvanceStatus_WithoutFee_Throws()
    {
        var documentRequestCase = DocumentRequestCase.Open(
            PersonId,
            DocumentRequestType.PassportRenewal,
            RequestedAt);

        documentRequestCase.AttachApplicantPhoto(AdministrativeDocumentId.New());

        var act = () => documentRequestCase.AdvanceStatus();

        act.Should().Throw<InvalidDocumentRequestTransitionException>()
            .WithMessage("*fee*");
    }

    [Fact]
    public void AdvanceStatus_FromReadyForCollection_Throws()
    {
        var documentRequestCase = CreateReadyForCollectionCase();

        var act = () => documentRequestCase.AdvanceStatus();

        act.Should().Throw<InvalidDocumentRequestTransitionException>();
    }

    [Fact]
    public void Issue_WithoutPhoto_Throws()
    {
        var documentRequestCase = CreateReadyForCollectionCase();
        documentRequestCase.RemoveApplicantPhoto();

        var act = () => documentRequestCase.Issue("BE-2026-0001", RequestedAt.AddHours(2));

        act.Should().Throw<InvalidDocumentRequestTransitionException>()
            .WithMessage("*photo*");
    }

    [Fact]
    public void Issue_WithPhoto_SetsIssuedStatus()
    {
        var documentRequestCase = CreateReadyForCollectionCase();

        documentRequestCase.Issue("BE-2026-0001", RequestedAt.AddHours(2));

        documentRequestCase.Status.Should().Be(DocumentRequestCaseStatus.Issued);
        documentRequestCase.IssuedDocumentNumber.Should().Be("BE-2026-0001");
    }

    [Fact]
    public void Cancel_FromNonTerminal_Allowed()
    {
        var documentRequestCase = DocumentRequestCase.Open(
            PersonId,
            DocumentRequestType.IdentityCard,
            RequestedAt);

        documentRequestCase.Cancel("Applicant withdrew", RequestedAt.AddHours(1));

        documentRequestCase.Status.Should().Be(DocumentRequestCaseStatus.Cancelled);
        documentRequestCase.CancellationReason.Should().Be("Applicant withdrew");
    }

    [Fact]
    public void Cancel_FromIssued_Throws()
    {
        var documentRequestCase = CreateReadyForCollectionCase();
        documentRequestCase.Issue("BE-2026-0001", RequestedAt.AddHours(2));

        var act = () => documentRequestCase.Cancel("Too late", RequestedAt.AddHours(3));

        act.Should().Throw<InvalidDocumentRequestTransitionException>();
    }

    [Fact]
    public void AdvanceStatus_FromTerminal_Throws()
    {
        var documentRequestCase = CreateReadyForCollectionCase();
        documentRequestCase.Issue("BE-2026-0001", RequestedAt.AddHours(2));

        var act = () => documentRequestCase.AdvanceStatus();

        act.Should().Throw<InvalidDocumentRequestTransitionException>();
    }

    private static DocumentRequestCase CreateReadyForCollectionCase()
    {
        var documentRequestCase = DocumentRequestCase.Open(
            PersonId,
            DocumentRequestType.IdentityCardRenewal,
            RequestedAt);

        documentRequestCase.AttachApplicantPhoto(AdministrativeDocumentId.New());
        documentRequestCase.RecordFeePayment("STUB-FEE");
        documentRequestCase.AdvanceStatus();
        documentRequestCase.AdvanceStatus();
        return documentRequestCase;
    }
}
