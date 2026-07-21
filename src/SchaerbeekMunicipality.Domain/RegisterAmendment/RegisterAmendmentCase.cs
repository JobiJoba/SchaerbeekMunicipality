using SchaerbeekMunicipality.Domain.CaseManagement;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.RegisterAmendment;

public sealed class RegisterAmendmentCase
{
    private RegisterAmendmentCase()
    {
        Checklist = RegisterAmendmentCaseChecklist.Empty();
    }

    public RegisterAmendmentCaseId Id { get; private set; }

    public PersonId PersonId { get; private set; }

    public AmendmentType AmendmentType { get; private set; }

    public RegisterAmendmentCaseStatus Status { get; private set; }

    public OfficerId? AssignedOfficerId { get; private set; }

    public OfficerId? LockedByOfficerId { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public string? Reason { get; private set; }

    public string? ProposedGivenName { get; private set; }

    public string? ProposedFamilyName { get; private set; }

    public string? ProposedNationality { get; private set; }

    public CivilStatusRecord? ProposedCivilStatus { get; private set; }

    public RegisterAmendmentCaseChecklist Checklist { get; private set; }

    public DateTimeOffset OpenedAt { get; private set; }

    public DateTimeOffset? SubmittedAt { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public DateTimeOffset? AppliedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public OfficerId? DecisionOfficerId { get; private set; }

    public OfficerId? AppliedByOfficerId { get; private set; }

    public RegisterAmendmentRejectionReason? RejectionReason { get; private set; }

    public string? DecisionNotes { get; private set; }

    public bool IsReadyForApproval =>
        Status == RegisterAmendmentCaseStatus.UnderReview &&
        Checklist.ProposedChangesRecorded &&
        Checklist.SupportingDocumentAttached;

    public static RegisterAmendmentCase Open(
        PersonId personId,
        AmendmentType amendmentType,
        DateTimeOffset openedAt)
    {
        return new RegisterAmendmentCase
        {
            Id = RegisterAmendmentCaseId.New(),
            PersonId = personId,
            AmendmentType = amendmentType,
            Status = RegisterAmendmentCaseStatus.Draft,
            OpenedAt = openedAt,
            Checklist = RegisterAmendmentCaseChecklist.Empty()
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
            message => new InvalidRegisterAmendmentTransitionException(message));

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
            message => new InvalidRegisterAmendmentTransitionException(message));

        LockedByOfficerId = outcome.LockedByOfficerId;
        LockedAt = outcome.LockedAt;
    }

    public void EnsureEditableBy(OfficerId officer, string operation)
    {
        OfficerCaseLocking.EnsureEditableBy(
            LockedByOfficerId,
            officer,
            operation,
            message => new InvalidRegisterAmendmentTransitionException(message));
    }

    public bool IsLockedTo(OfficerId officer)
    {
        return OfficerCaseLocking.IsLockedTo(LockedByOfficerId, officer);
    }

    public bool IsLockedToAnother(OfficerId officer)
    {
        return OfficerCaseLocking.IsLockedToAnother(LockedByOfficerId, officer);
    }

    public void SetReason(string? reason)
    {
        EnsureDraftEditable(nameof(SetReason));
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    public void RecordIdentityCorrection(string givenName, string familyName)
    {
        EnsureDraftEditable(nameof(RecordIdentityCorrection));

        if (AmendmentType != AmendmentType.IdentityCorrection)
            throw new InvalidRegisterAmendmentTransitionException(
                "Identity correction values can only be recorded for identity correction amendments.");

        ArgumentException.ThrowIfNullOrWhiteSpace(givenName);
        ArgumentException.ThrowIfNullOrWhiteSpace(familyName);

        ProposedGivenName = givenName.Trim();
        ProposedFamilyName = familyName.Trim();
        Checklist.MarkProposedChangesRecorded();
    }

    public void RecordNationalityChange(string nationality)
    {
        EnsureDraftEditable(nameof(RecordNationalityChange));

        if (AmendmentType != AmendmentType.NationalityChange)
            throw new InvalidRegisterAmendmentTransitionException(
                "Nationality change values can only be recorded for nationality change amendments.");

        ArgumentException.ThrowIfNullOrWhiteSpace(nationality);

        ProposedNationality = nationality.Trim();
        Checklist.MarkProposedChangesRecorded();
    }

    public void RecordCivilStatusUpdate(CivilStatusDetails details)
    {
        EnsureDraftEditable(nameof(RecordCivilStatusUpdate));

        if (AmendmentType != AmendmentType.CivilStatusUpdate)
            throw new InvalidRegisterAmendmentTransitionException(
                "Civil status values can only be recorded for civil status update amendments.");

        ProposedCivilStatus = CivilStatusRecord.Create(details);
        Checklist.MarkProposedChangesRecorded();
    }

    public void MarkSupportingDocumentAttached()
    {
        EnsureDraftEditable(nameof(MarkSupportingDocumentAttached));
        Checklist.MarkSupportingDocumentAttached();
    }

    public void ClearSupportingDocumentAttached()
    {
        EnsureDraftEditable(nameof(ClearSupportingDocumentAttached));
        Checklist.ClearSupportingDocumentAttached();
    }

    public void SubmitForReview(DateTimeOffset submittedAt)
    {
        EnsureDraftEditable(nameof(SubmitForReview));

        if (!Checklist.ProposedChangesRecorded)
            throw new InvalidRegisterAmendmentTransitionException(
                "Proposed changes must be recorded before submitting for review.");

        if (!Checklist.SupportingDocumentAttached)
            throw new InvalidRegisterAmendmentTransitionException(
                "At least one supporting document must be attached before submitting for review.");

        Status = RegisterAmendmentCaseStatus.UnderReview;
        SubmittedAt = submittedAt;
    }

    public void Approve(OfficerId officer, string? notes, DateTimeOffset approvedAt)
    {
        EnsureStatus(RegisterAmendmentCaseStatus.UnderReview, nameof(Approve));

        if (!IsReadyForApproval)
            throw new InvalidRegisterAmendmentTransitionException(
                "The amendment case is not ready for approval.");

        DecisionOfficerId = officer;
        DecisionNotes = notes?.Trim();
        Status = RegisterAmendmentCaseStatus.Approved;
        ApprovedAt = approvedAt;
    }

    public void Reject(
        OfficerId officer,
        RegisterAmendmentRejectionReason reason,
        string? notes,
        DateTimeOffset rejectedAt)
    {
        if (Status is not (RegisterAmendmentCaseStatus.Draft or RegisterAmendmentCaseStatus.UnderReview))
            throw new InvalidRegisterAmendmentTransitionException(
                $"Cannot perform '{nameof(Reject)}' while the case is in status '{Status}'.");

        DecisionOfficerId = officer;
        RejectionReason = reason;
        DecisionNotes = notes?.Trim();
        Status = RegisterAmendmentCaseStatus.Rejected;
        ClosedAt = rejectedAt;
    }

    public AmendmentAppliedEventDetails Apply(Person person, OfficerId officer, DateTimeOffset appliedAt)
    {
        EnsureStatus(RegisterAmendmentCaseStatus.Approved, nameof(Apply));

        if (person.Id != PersonId)
            throw new InvalidRegisterAmendmentTransitionException(
                "The person does not belong to this amendment case.");

        var summary = ApplyChangesToPerson(person);

        AppliedByOfficerId = officer;
        Status = RegisterAmendmentCaseStatus.Applied;
        AppliedAt = appliedAt;
        ClosedAt = appliedAt;

        return new AmendmentAppliedEventDetails(Id, PersonId, AmendmentType, summary, appliedAt);
    }

    public string BuildChangeSummary()
    {
        return AmendmentType switch
        {
            AmendmentType.IdentityCorrection =>
                $"Name: {ProposedGivenName} {ProposedFamilyName}",
            AmendmentType.NationalityChange =>
                $"Nationality: {ProposedNationality}",
            AmendmentType.CivilStatusUpdate =>
                $"Civil status: {ProposedCivilStatus?.Status}",
            _ => AmendmentType.ToString()
        };
    }

    private string ApplyChangesToPerson(Person person)
    {
        return AmendmentType switch
        {
            AmendmentType.IdentityCorrection => ApplyIdentityCorrection(person),
            AmendmentType.NationalityChange => ApplyNationalityChange(person),
            AmendmentType.CivilStatusUpdate => ApplyCivilStatusUpdate(person),
            _ => throw new InvalidRegisterAmendmentTransitionException(
                $"Unsupported amendment type '{AmendmentType}'.")
        };
    }

    private string ApplyIdentityCorrection(Person person)
    {
        if (ProposedGivenName is null || ProposedFamilyName is null)
            throw new InvalidRegisterAmendmentTransitionException(
                "Proposed identity values are missing.");

        person.Update(new IdentityDetails(
            ProposedGivenName,
            ProposedFamilyName,
            person.BirthDate,
            person.Nationality));

        return BuildChangeSummary();
    }

    private string ApplyNationalityChange(Person person)
    {
        if (ProposedNationality is null)
            throw new InvalidRegisterAmendmentTransitionException(
                "Proposed nationality is missing.");

        person.Update(new IdentityDetails(
            person.GivenName,
            person.FamilyName,
            person.BirthDate,
            ProposedNationality));

        return BuildChangeSummary();
    }

    private string ApplyCivilStatusUpdate(Person person)
    {
        if (ProposedCivilStatus is null)
            throw new InvalidRegisterAmendmentTransitionException(
                "Proposed civil status is missing.");

        person.RecordCivilStatus(new CivilStatusDetails(
            ProposedCivilStatus.Status,
            ProposedCivilStatus.SpouseGivenName,
            ProposedCivilStatus.SpouseFamilyName,
            ProposedCivilStatus.MarriageDate,
            ProposedCivilStatus.MarriagePlace,
            ProposedCivilStatus.MarriageRecognitionStatus));

        return BuildChangeSummary();
    }

    private void EnsureDraftEditable(string operation)
    {
        if (Status != RegisterAmendmentCaseStatus.Draft)
            throw new InvalidRegisterAmendmentTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
    }

    private void EnsureStatus(RegisterAmendmentCaseStatus requiredStatus, string operation)
    {
        if (Status != requiredStatus)
            throw new InvalidRegisterAmendmentTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
    }
}

public sealed record AmendmentAppliedEventDetails(
    RegisterAmendmentCaseId CaseId,
    PersonId PersonId,
    AmendmentType AmendmentType,
    string ChangeSummary,
    DateTimeOffset AppliedAt);
