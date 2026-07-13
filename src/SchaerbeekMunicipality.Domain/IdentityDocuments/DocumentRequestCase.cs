using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.CaseManagement;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.IdentityDocuments;

public sealed class DocumentRequestCase
{
    private DocumentRequestCase()
    {
    }

    public DocumentRequestCaseId Id { get; private set; }

    public PersonId PersonId { get; private set; }

    public DocumentRequestType RequestType { get; private set; }

    public DocumentRequestCaseStatus Status { get; private set; }

    public OfficerId? AssignedOfficerId { get; private set; }

    public OfficerId? LockedByOfficerId { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public AdministrativeDocumentId? PhotoDocumentId { get; private set; }

    public bool PhotoAttached { get; private set; }

    public bool FeePaid { get; private set; }

    public string? FeePaymentReference { get; private set; }

    public string? IssuedDocumentNumber { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset? IssuedAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public static DocumentRequestCase Open(
        PersonId personId,
        DocumentRequestType requestType,
        DateTimeOffset requestedAt)
    {
        return new DocumentRequestCase
        {
            Id = DocumentRequestCaseId.New(),
            PersonId = personId,
            RequestType = requestType,
            Status = DocumentRequestCaseStatus.Submitted,
            RequestedAt = requestedAt,
        };
    }

    public CaseClaimResult Claim(OfficerId officer, DateTimeOffset at)
    {
        var outcome = OfficerCaseLocking.Claim(
            AssignedOfficerId,
            LockedByOfficerId,
            LockedAt,
            officer,
            at,
            message => new InvalidDocumentRequestTransitionException(message));

        AssignedOfficerId = outcome.AssignedOfficerId;
        LockedByOfficerId = outcome.LockedByOfficerId;
        LockedAt = outcome.LockedAt;

        return outcome.Result;
    }

    public void ReleaseLock(OfficerId officer)
    {
        var outcome = OfficerCaseLocking.ReleaseLock(
            LockedByOfficerId,
            officer,
            message => new InvalidDocumentRequestTransitionException(message));

        LockedByOfficerId = outcome.LockedByOfficerId;
        LockedAt = outcome.LockedAt;
    }

    public void EnsureEditableBy(OfficerId officer, string operation)
    {
        OfficerCaseLocking.EnsureEditableBy(
            LockedByOfficerId,
            officer,
            operation,
            message => new InvalidDocumentRequestTransitionException(message));
    }

    public bool IsLockedTo(OfficerId officer) =>
        OfficerCaseLocking.IsLockedTo(LockedByOfficerId, officer);

    public bool IsLockedToAnother(OfficerId officer) =>
        OfficerCaseLocking.IsLockedToAnother(LockedByOfficerId, officer);

    public void AttachApplicantPhoto(AdministrativeDocumentId documentId)
    {
        EnsureNotTerminal(nameof(AttachApplicantPhoto));
        PhotoDocumentId = documentId;
        PhotoAttached = true;
    }

    public void RemoveApplicantPhoto()
    {
        EnsureNotTerminal(nameof(RemoveApplicantPhoto));
        PhotoDocumentId = null;
        PhotoAttached = false;
    }

    public void RecordFeePayment(string? reference)
    {
        EnsureNotTerminal(nameof(RecordFeePayment));
        FeePaid = true;
        FeePaymentReference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
    }

    public void AdvanceStatus()
    {
        DocumentRequestRules.EnsureCanAdvance(Status, PhotoAttached, FeePaid);
        Status = DocumentRequestRules.NextStatus(Status);
    }

    public void Issue(string documentNumber, DateTimeOffset issuedAt)
    {
        DocumentRequestRules.EnsureCanIssue(Status, PhotoAttached, FeePaid);

        ArgumentException.ThrowIfNullOrWhiteSpace(documentNumber);

        Status = DocumentRequestCaseStatus.Issued;
        IssuedDocumentNumber = documentNumber.Trim();
        IssuedAt = issuedAt;
    }

    public void Cancel(string reason, DateTimeOffset cancelledAt)
    {
        DocumentRequestRules.EnsureCanCancel(Status);

        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        Status = DocumentRequestCaseStatus.Cancelled;
        CancellationReason = reason.Trim();
        CancelledAt = cancelledAt;
    }

    private void EnsureNotTerminal(string operation)
    {
        if (Status is DocumentRequestCaseStatus.Issued or DocumentRequestCaseStatus.Cancelled)
        {
            throw new InvalidDocumentRequestTransitionException(
                $"Cannot perform '{operation}' on a request in '{Status}' status.");
        }
    }
}
