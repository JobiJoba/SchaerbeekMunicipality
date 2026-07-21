using SchaerbeekMunicipality.Domain.CaseManagement;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public sealed class DeathDeclarationCase
{
    private DeathDeclarationCase()
    {
        Checklist = DeathDeclarationCaseChecklist.Empty();
    }

    public DeathDeclarationCaseId Id { get; private set; }

    public PersonId PersonId { get; private set; }

    public DeathDeclarationCaseStatus Status { get; private set; }

    public OfficerId? AssignedOfficerId { get; private set; }

    public OfficerId? LockedByOfficerId { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public DateOnly? DeathDate { get; private set; }

    public string? DeathPlace { get; private set; }

    public bool DeathAbroad { get; private set; }

    public InformantRelationship? InformantRelationship { get; private set; }

    public AdministrativeDocumentId? DeathActDocumentId { get; private set; }

    public DateTimeOffset? HouseholdReviewedAt { get; private set; }

    public DeathDeclarationCaseChecklist Checklist { get; private set; }

    public DateTimeOffset OpenedAt { get; private set; }

    public DateTimeOffset? ConfirmedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public OfficerId? DecisionOfficerId { get; private set; }

    public DeathDeclarationRejectionReason? RejectionReason { get; private set; }

    public SuspensionReason? SuspensionReason { get; private set; }

    public string? DecisionNotes { get; private set; }

    public DeathDeclarationCaseStatus? StatusBeforeSuspension { get; private set; }

    public bool IsReadyForConfirmation =>
        Status is DeathDeclarationCaseStatus.Intake or DeathDeclarationCaseStatus.UnderReview &&
        Checklist.PersonIdentified &&
        Checklist.DeathFactsRecorded &&
        Checklist.DeathActAttached &&
        Checklist.HouseholdReviewed;

    public static DeathDeclarationCase Open(PersonId personId, DateTimeOffset openedAt)
    {
        var checklist = DeathDeclarationCaseChecklist.Empty();
        checklist.MarkPersonIdentified();

        return new DeathDeclarationCase
        {
            Id = DeathDeclarationCaseId.New(),
            PersonId = personId,
            Status = DeathDeclarationCaseStatus.Intake,
            OpenedAt = openedAt,
            Checklist = checklist
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
            message => new InvalidDeathDeclarationTransitionException(message));

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
            message => new InvalidDeathDeclarationTransitionException(message));

        LockedByOfficerId = outcome.LockedByOfficerId;
        LockedAt = outcome.LockedAt;
    }

    public void EnsureEditableBy(OfficerId officer, string operation)
    {
        OfficerCaseLocking.EnsureEditableBy(
            LockedByOfficerId,
            officer,
            operation,
            message => new InvalidDeathDeclarationTransitionException(message));
    }

    public bool IsLockedTo(OfficerId officer)
    {
        return OfficerCaseLocking.IsLockedTo(LockedByOfficerId, officer);
    }

    public bool IsLockedToAnother(OfficerId officer)
    {
        return OfficerCaseLocking.IsLockedToAnother(LockedByOfficerId, officer);
    }

    public void RecordDeathFacts(DeathFacts facts, DateOnly today)
    {
        EnsureIntakeDataEditable(nameof(RecordDeathFacts));

        ArgumentNullException.ThrowIfNull(facts);
        ArgumentException.ThrowIfNullOrWhiteSpace(facts.DeathPlace);

        if (facts.DeathDate > today)
            throw new InvalidDeathDeclarationTransitionException(
                "Date of death cannot be in the future.");

        DeathDate = facts.DeathDate;
        DeathPlace = facts.DeathPlace.Trim();
        DeathAbroad = facts.DeathAbroad;
        InformantRelationship = facts.InformantRelationship;
        Checklist.MarkDeathFactsRecorded();
        MoveToUnderReviewIfReady();
    }

    public void AttachDeathAct(AdministrativeDocumentId documentId)
    {
        EnsureIntakeDataEditable(nameof(AttachDeathAct));

        DeathActDocumentId = documentId;
        Checklist.MarkDeathActAttached();
        MoveToUnderReviewIfReady();
    }

    public void RemoveDeathAct()
    {
        EnsureIntakeDataEditable(nameof(RemoveDeathAct));

        DeathActDocumentId = null;
        Checklist.ClearDeathActAttached();
    }

    public void ReviewHousehold(DateTimeOffset reviewedAt)
    {
        EnsureIntakeDataEditable(nameof(ReviewHousehold));

        HouseholdReviewedAt = reviewedAt;
        Checklist.MarkHouseholdReviewed();
        MoveToUnderReviewIfReady();
    }

    public PersonRadiatedEventDetails ConfirmRadiation(DateTimeOffset confirmedAt)
    {
        if (Status is not (DeathDeclarationCaseStatus.Intake or DeathDeclarationCaseStatus.UnderReview))
            throw new InvalidDeathDeclarationTransitionException(
                $"Cannot perform '{nameof(ConfirmRadiation)}' while the case is in status '{Status}'.");

        if (!IsReadyForConfirmation)
            throw new InvalidDeathDeclarationTransitionException(
                "Cannot confirm the radiation until all checklist items are satisfied.");

        if (DeathActDocumentId is null)
            throw new InvalidDeathDeclarationTransitionException(
                "A death act must be attached before confirmation.");

        if (DeathDate is null)
            throw new InvalidDeathDeclarationTransitionException(
                "Death facts must be recorded before confirmation.");

        Status = DeathDeclarationCaseStatus.Confirmed;
        ConfirmedAt = confirmedAt;
        ClosedAt = confirmedAt;

        return new PersonRadiatedEventDetails(Id, PersonId, DeathDate.Value, confirmedAt);
    }

    public void Reject(
        OfficerId officer,
        DeathDeclarationRejectionReason reason,
        string? notes,
        DateTimeOffset rejectedAt)
    {
        if (Status is not (DeathDeclarationCaseStatus.Intake or DeathDeclarationCaseStatus.UnderReview))
            throw new InvalidDeathDeclarationTransitionException(
                $"Cannot perform '{nameof(Reject)}' while the case is in status '{Status}'.");

        DecisionOfficerId = officer;
        RejectionReason = reason;
        SuspensionReason = null;
        DecisionNotes = notes?.Trim();
        Status = DeathDeclarationCaseStatus.Rejected;
        StatusBeforeSuspension = null;
        ClosedAt = rejectedAt;
    }

    public void Suspend(
        OfficerId officer,
        SuspensionReason reason,
        string? notes,
        DateTimeOffset suspendedAt)
    {
        if (Status is not (DeathDeclarationCaseStatus.Intake or DeathDeclarationCaseStatus.UnderReview))
            throw new InvalidDeathDeclarationTransitionException(
                $"Cannot suspend the case while it is in status '{Status}'.");

        _ = suspendedAt;
        StatusBeforeSuspension = Status;
        DecisionOfficerId = officer;
        SuspensionReason = reason;
        RejectionReason = null;
        DecisionNotes = notes?.Trim();
        Status = DeathDeclarationCaseStatus.Suspended;
        ClosedAt = null;
    }

    public void ResumeFromSuspension()
    {
        EnsureStatus(DeathDeclarationCaseStatus.Suspended, nameof(ResumeFromSuspension));

        Status = StatusBeforeSuspension ?? DeathDeclarationCaseStatus.Intake;
        StatusBeforeSuspension = null;
        SuspensionReason = null;
        DecisionNotes = null;
        DecisionOfficerId = null;
    }

    public void EnsureIntakeDataEditable(string operation)
    {
        if (Status is not (DeathDeclarationCaseStatus.Intake or DeathDeclarationCaseStatus.UnderReview))
            throw new InvalidDeathDeclarationTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
    }

    private void MoveToUnderReviewIfReady()
    {
        if (Status == DeathDeclarationCaseStatus.Intake && Checklist.DeathFactsRecorded)
            Status = DeathDeclarationCaseStatus.UnderReview;
    }

    private void EnsureStatus(DeathDeclarationCaseStatus requiredStatus, string operation)
    {
        if (Status != requiredStatus)
            throw new InvalidDeathDeclarationTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
    }
}
