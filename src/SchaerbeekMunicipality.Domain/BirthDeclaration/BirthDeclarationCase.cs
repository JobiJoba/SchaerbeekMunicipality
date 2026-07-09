using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public sealed class BirthDeclarationCase
{
    private readonly List<BirthDeclarationParentLink> _parentLinks = [];

    private BirthDeclarationCase()
    {
        Checklist = BirthDeclarationCaseChecklist.Empty();
    }

    public BirthDeclarationCaseId Id { get; private set; }

    public BirthDeclarationCaseStatus Status { get; private set; }

    public OfficerId? AssignedOfficerId { get; private set; }

    public OfficerId? LockedByOfficerId { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public string? ChildGivenNames { get; private set; }

    public string? ChildFamilyName { get; private set; }

    public NewbornSex? ChildSex { get; private set; }

    public DateOnly? ChildDateOfBirth { get; private set; }

    public TimeOnly? ChildTimeOfBirth { get; private set; }

    public string? ChildPlaceOfBirth { get; private set; }

    public IReadOnlyList<BirthDeclarationParentLink> ParentLinks => _parentLinks;

    public AdministrativeDocumentId? MedicalDeclarationDocumentId { get; private set; }

    public BelgianAddress? HouseholdAddress { get; private set; }

    public BirthDeclarationCaseChecklist Checklist { get; private set; }

    public DateTimeOffset OpenedAt { get; private set; }

    public DateTimeOffset? ConfirmedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public PersonId? ChildPersonId { get; private set; }

    public string? ChildNationalRegisterNumber { get; private set; }

    public OfficerId? DecisionOfficerId { get; private set; }

    public BirthDeclarationRejectionReason? RejectionReason { get; private set; }

    public SuspensionReason? SuspensionReason { get; private set; }

    public string? DecisionNotes { get; private set; }

    public BirthDeclarationCaseStatus? StatusBeforeSuspension { get; private set; }

    public static BirthDeclarationCase Open(DateTimeOffset openedAt)
    {
        return new BirthDeclarationCase
        {
            Id = BirthDeclarationCaseId.New(),
            Status = BirthDeclarationCaseStatus.Intake,
            OpenedAt = openedAt,
            Checklist = BirthDeclarationCaseChecklist.Empty(),
        };
    }

    public CaseClaimResult Claim(OfficerId officer, DateTimeOffset at)
    {
        if (LockedByOfficerId is { } locked && locked != officer)
        {
            throw new InvalidBirthDeclarationTransitionException(
                "This case is locked to another officer.");
        }

        if (LockedByOfficerId == officer)
        {
            return CaseClaimResult.AlreadyHeld;
        }

        var hadAssignee = AssignedOfficerId is not null;
        AssignedOfficerId = officer;
        LockedByOfficerId = officer;
        LockedAt = at;

        return hadAssignee ? CaseClaimResult.Reclaimed : CaseClaimResult.NewlyClaimed;
    }

    public void ReleaseLock(OfficerId officer)
    {
        if (LockedByOfficerId != officer)
        {
            throw new InvalidBirthDeclarationTransitionException(
                "Only the officer holding the lock can release it.");
        }

        LockedByOfficerId = null;
        LockedAt = null;
    }

    public void EnsureEditableBy(OfficerId officer, string operation)
    {
        if (LockedByOfficerId is null)
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"Cannot perform '{operation}' before the case is claimed.");
        }

        if (LockedByOfficerId != officer)
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"Cannot perform '{operation}' while the case is locked to another officer.");
        }
    }

    public bool IsLockedTo(OfficerId officer) => LockedByOfficerId == officer;

    public bool IsLockedToAnother(OfficerId officer) =>
        LockedByOfficerId is { } locked && locked != officer;

    public void RecordChildDetails(NewbornDetails details, DateOnly today)
    {
        EnsureIntakeDataEditable(nameof(RecordChildDetails));

        if (details.DateOfBirth > today)
        {
            throw new InvalidBirthDeclarationTransitionException(
                "Date of birth cannot be in the future.");
        }

        if (today.DayNumber - details.DateOfBirth.DayNumber > BirthDeclarationRules.DeclarationWindowDays)
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"Birth must be declared within {BirthDeclarationRules.DeclarationWindowDays} days.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(details.GivenNames);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.FamilyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.PlaceOfBirth);

        ChildGivenNames = details.GivenNames.Trim();
        ChildFamilyName = details.FamilyName.Trim();
        ChildSex = details.Sex;
        ChildDateOfBirth = details.DateOfBirth;
        ChildTimeOfBirth = details.TimeOfBirth;
        ChildPlaceOfBirth = details.PlaceOfBirth.Trim();
        Checklist.MarkChildDetailsRecorded();
        MoveToUnderReviewIfReady();
    }

    public void LinkParent(PersonId personId, ParentRole role)
    {
        EnsureIntakeDataEditable(nameof(LinkParent));

        if (_parentLinks.Any(link => link.PersonId == personId))
        {
            throw new InvalidBirthDeclarationTransitionException(
                "This parent is already linked to the case.");
        }

        if (_parentLinks.Any(link => link.Role == role))
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"A parent with role '{role}' is already linked.");
        }

        _parentLinks.Add(BirthDeclarationParentLink.Create(personId, role));
        Checklist.MarkAtLeastOneParentLinked();
        MoveToUnderReviewIfReady();
    }

    public void UnlinkParent(PersonId personId)
    {
        EnsureIntakeDataEditable(nameof(UnlinkParent));

        var removed = _parentLinks.RemoveAll(link => link.PersonId == personId);
        if (removed == 0)
        {
            throw new InvalidBirthDeclarationTransitionException(
                "The parent is not linked to this case.");
        }

        if (_parentLinks.Count == 0)
        {
            Checklist.ClearAtLeastOneParentLinked();
        }
    }

    public void AttachMedicalDeclaration(AdministrativeDocumentId documentId)
    {
        EnsureIntakeDataEditable(nameof(AttachMedicalDeclaration));

        MedicalDeclarationDocumentId = documentId;
        Checklist.MarkMedicalDeclarationAttached();
        MoveToUnderReviewIfReady();
    }

    public void RemoveMedicalDeclaration()
    {
        EnsureIntakeDataEditable(nameof(RemoveMedicalDeclaration));

        MedicalDeclarationDocumentId = null;
        Checklist.ClearMedicalDeclarationAttached();
    }

    public void SetHousehold(BelgianAddress address)
    {
        EnsureIntakeDataEditable(nameof(SetHousehold));

        HouseholdAddress = address;
        Checklist.MarkHouseholdEstablished();
        MoveToUnderReviewIfReady();
    }

    public bool IsReadyForConfirmation =>
        Status is BirthDeclarationCaseStatus.Intake or BirthDeclarationCaseStatus.UnderReview &&
        Checklist.ChildDetailsRecorded &&
        Checklist.AtLeastOneParentLinked &&
        Checklist.MedicalDeclarationAttached &&
        Checklist.HouseholdEstablished;

    public BirthRegisteredEventDetails ConfirmDeclaration(
        PersonId childPersonId,
        string childNationalRegisterNumber,
        DateTimeOffset confirmedAt)
    {
        if (Status is not (BirthDeclarationCaseStatus.Intake or BirthDeclarationCaseStatus.UnderReview))
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"Cannot perform '{nameof(ConfirmDeclaration)}' while the case is in status '{Status}'.");
        }

        if (!IsReadyForConfirmation)
        {
            throw new InvalidBirthDeclarationTransitionException(
                "Cannot confirm the declaration until all checklist items are satisfied.");
        }

        if (MedicalDeclarationDocumentId is null)
        {
            throw new InvalidBirthDeclarationTransitionException(
                "A medical birth declaration must be attached before confirmation.");
        }

        if (_parentLinks.Count == 0)
        {
            throw new InvalidBirthDeclarationTransitionException(
                "At least one parent must be linked before confirmation.");
        }

        Status = BirthDeclarationCaseStatus.Confirmed;
        ChildPersonId = childPersonId;
        ChildNationalRegisterNumber = childNationalRegisterNumber;
        ConfirmedAt = confirmedAt;
        ClosedAt = confirmedAt;

        return new BirthRegisteredEventDetails(
            Id,
            childPersonId,
            childNationalRegisterNumber,
            confirmedAt);
    }

    public void Reject(
        OfficerId officer,
        BirthDeclarationRejectionReason reason,
        string? notes,
        DateTimeOffset rejectedAt)
    {
        EnsureStatus(BirthDeclarationCaseStatus.UnderReview, nameof(Reject));

        DecisionOfficerId = officer;
        RejectionReason = reason;
        SuspensionReason = null;
        DecisionNotes = notes?.Trim();
        Status = BirthDeclarationCaseStatus.Rejected;
        StatusBeforeSuspension = null;
        ClosedAt = rejectedAt;
    }

    public void Suspend(
        OfficerId officer,
        SuspensionReason reason,
        string? notes,
        DateTimeOffset suspendedAt)
    {
        if (Status is not (BirthDeclarationCaseStatus.Intake or BirthDeclarationCaseStatus.UnderReview))
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"Cannot suspend the case while it is in status '{Status}'.");
        }

        _ = suspendedAt;
        StatusBeforeSuspension = Status;
        DecisionOfficerId = officer;
        SuspensionReason = reason;
        RejectionReason = null;
        DecisionNotes = notes?.Trim();
        Status = BirthDeclarationCaseStatus.Suspended;
        ClosedAt = null;
    }

    public void ResumeFromSuspension()
    {
        EnsureStatus(BirthDeclarationCaseStatus.Suspended, nameof(ResumeFromSuspension));

        Status = StatusBeforeSuspension ?? BirthDeclarationCaseStatus.Intake;
        StatusBeforeSuspension = null;
        SuspensionReason = null;
        DecisionNotes = null;
        DecisionOfficerId = null;
    }

    public void EnsureIntakeDataEditable(string operation)
    {
        if (Status is not (BirthDeclarationCaseStatus.Intake or BirthDeclarationCaseStatus.UnderReview))
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
        }
    }

    private void MoveToUnderReviewIfReady()
    {
        if (Status == BirthDeclarationCaseStatus.Intake &&
            Checklist.ChildDetailsRecorded &&
            Checklist.AtLeastOneParentLinked)
        {
            Status = BirthDeclarationCaseStatus.UnderReview;
        }
    }

    private void EnsureStatus(BirthDeclarationCaseStatus requiredStatus, string operation)
    {
        if (Status != requiredStatus)
        {
            throw new InvalidBirthDeclarationTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
        }
    }
}
