using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.CaseManagement;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public sealed class ChangeOfAddressCase
{
    private readonly List<ChangeOfAddressHouseholdMemberLink> _householdMemberLinks = [];

    private ChangeOfAddressCase()
    {
        Checklist = ChangeOfAddressCaseChecklist.Empty();
    }

    public ChangeOfAddressCaseId Id { get; private set; }

    public PersonId PersonId { get; private set; }

    public ChangeOfAddressCaseStatus Status { get; private set; }

    public OfficerId? AssignedOfficerId { get; private set; }

    public OfficerId? LockedByOfficerId { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public BelgianAddress? PreviousAddress { get; private set; }

    public BelgianAddress? NewAddress { get; private set; }

    public HousingSituation? HousingSituation { get; private set; }

    public AdministrativeDocumentId? HousingDocumentId { get; private set; }

    public PoliceVerificationRequestId? PoliceVerificationRequestId { get; private set; }

    public DateOnly? EffectiveDate { get; private set; }

    public IReadOnlyList<ChangeOfAddressHouseholdMemberLink> HouseholdMemberLinks => _householdMemberLinks;

    public ChangeOfAddressCaseChecklist Checklist { get; private set; }

    public DateTimeOffset OpenedAt { get; private set; }

    public DateTimeOffset? ConfirmedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public OfficerId? DecisionOfficerId { get; private set; }

    public ChangeOfAddressRejectionReason? RejectionReason { get; private set; }

    public string? DecisionNotes { get; private set; }

    public bool IsReadyForConfirmation =>
        Status is ChangeOfAddressCaseStatus.Intake or ChangeOfAddressCaseStatus.UnderReview &&
        Checklist.PersonIdentified &&
        Checklist.NewAddressDeclared &&
        (!Checklist.HousingDocumentRequired || Checklist.HousingDocumentAttached) &&
        (!Checklist.PoliceVerificationRequested || Checklist.PoliceVerificationPositive);

    public static ChangeOfAddressCase Open(
        PersonId personId,
        BelgianAddress? previousAddress,
        DateTimeOffset openedAt)
    {
        var checklist = ChangeOfAddressCaseChecklist.Empty();
        checklist.MarkPersonIdentified();

        return new ChangeOfAddressCase
        {
            Id = ChangeOfAddressCaseId.New(),
            PersonId = personId,
            Status = ChangeOfAddressCaseStatus.Intake,
            PreviousAddress = previousAddress,
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
            message => new InvalidChangeOfAddressTransitionException(message));

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
            message => new InvalidChangeOfAddressTransitionException(message));

        LockedByOfficerId = outcome.LockedByOfficerId;
        LockedAt = outcome.LockedAt;
    }

    public void EnsureEditableBy(OfficerId officer, string operation)
    {
        OfficerCaseLocking.EnsureEditableBy(
            LockedByOfficerId,
            officer,
            operation,
            message => new InvalidChangeOfAddressTransitionException(message));
    }

    public bool IsLockedTo(OfficerId officer)
    {
        return OfficerCaseLocking.IsLockedTo(LockedByOfficerId, officer);
    }

    public bool IsLockedToAnother(OfficerId officer)
    {
        return OfficerCaseLocking.IsLockedToAnother(LockedByOfficerId, officer);
    }

    public void DeclareNewAddress(
        BelgianAddress address,
        HousingSituation housingSituation,
        DateOnly effectiveDate)
    {
        EnsureIntakeDataEditable(nameof(DeclareNewAddress));

        if (!ChangeOfAddressRules.IsValidMunicipalityAddress(address))
            throw new InvalidChangeOfAddressTransitionException(
                "The new address must be within the municipality of Schaerbeek (1030).");

        NewAddress = address;
        HousingSituation = housingSituation;
        EffectiveDate = effectiveDate;
        Checklist.MarkNewAddressDeclared();

        var requiresDocument = ChangeOfAddressRules.RequiresHousingDocument(housingSituation);
        Checklist.SetHousingDocumentRequired(requiresDocument);

        if (!requiresDocument)
            Checklist.MarkHousingDocumentAttached();
        else
            Checklist.ClearHousingDocumentAttached();

        MoveToUnderReviewIfReady();
    }

    public void AttachHousingDocument(AdministrativeDocumentId documentId)
    {
        EnsureIntakeDataEditable(nameof(AttachHousingDocument));

        HousingDocumentId = documentId;
        Checklist.MarkHousingDocumentAttached();
        MoveToUnderReviewIfReady();
    }

    public void RemoveHousingDocument()
    {
        EnsureIntakeDataEditable(nameof(RemoveHousingDocument));

        HousingDocumentId = null;

        if (Checklist.HousingDocumentRequired) Checklist.ClearHousingDocumentAttached();
    }

    public void AddHouseholdMember(PersonId personId)
    {
        EnsureIntakeDataEditable(nameof(AddHouseholdMember));

        if (personId == PersonId)
            throw new InvalidChangeOfAddressTransitionException(
                "The subject of the case is already the primary resident.");

        if (_householdMemberLinks.Any(link => link.PersonId == personId))
            throw new InvalidChangeOfAddressTransitionException(
                "This person is already linked to the household move.");

        _householdMemberLinks.Add(ChangeOfAddressHouseholdMemberLink.Create(personId));
    }

    public void RemoveHouseholdMember(PersonId personId)
    {
        EnsureIntakeDataEditable(nameof(RemoveHouseholdMember));

        var removed = _householdMemberLinks.RemoveAll(link => link.PersonId == personId);
        if (removed == 0)
            throw new InvalidChangeOfAddressTransitionException(
                "The person is not linked to this household move.");
    }

    public void RequestPoliceVerification()
    {
        EnsureIntakeDataEditable(nameof(RequestPoliceVerification));

        if (NewAddress is null)
            throw new InvalidChangeOfAddressTransitionException(
                "A new address must be declared before requesting police verification.");

        if (PoliceVerificationRequestId is not null && Status == ChangeOfAddressCaseStatus.AwaitingPoliceVerification)
            throw new InvalidChangeOfAddressTransitionException(
                "A police verification request is already pending for this case.");

        Checklist.MarkPoliceVerificationRequested();
        Checklist.ClearPoliceVerificationPositive();
        Status = ChangeOfAddressCaseStatus.AwaitingPoliceVerification;
    }

    public void LinkPoliceVerificationRequest(PoliceVerificationRequestId requestId)
    {
        PoliceVerificationRequestId = requestId;
    }

    public void ApplyPoliceVerificationResult(PoliceVerificationResult result)
    {
        EnsureStatus(ChangeOfAddressCaseStatus.AwaitingPoliceVerification, nameof(ApplyPoliceVerificationResult));

        if (result == PoliceVerificationResult.Confirmed)
        {
            Checklist.MarkPoliceVerificationPositive();
            Status = ChangeOfAddressCaseStatus.UnderReview;
        }
        else
        {
            Checklist.ClearPoliceVerificationPositive();
            Status = ChangeOfAddressCaseStatus.UnderReview;
        }
    }

    public AddressChangedEventDetails ConfirmAddressChange(DateTimeOffset confirmedAt)
    {
        if (Status is not (ChangeOfAddressCaseStatus.Intake or ChangeOfAddressCaseStatus.UnderReview))
            throw new InvalidChangeOfAddressTransitionException(
                $"Cannot perform '{nameof(ConfirmAddressChange)}' while the case is in status '{Status}'.");

        if (Status == ChangeOfAddressCaseStatus.AwaitingPoliceVerification)
            throw new InvalidChangeOfAddressTransitionException(
                "Cannot confirm the address change while police verification is pending.");

        if (!IsReadyForConfirmation)
            throw new InvalidChangeOfAddressTransitionException(
                "Cannot confirm the address change until all checklist items are satisfied.");

        if (NewAddress is null)
            throw new InvalidChangeOfAddressTransitionException(
                "A new address must be declared before confirmation.");

        Status = ChangeOfAddressCaseStatus.Confirmed;
        ConfirmedAt = confirmedAt;
        ClosedAt = confirmedAt;

        return new AddressChangedEventDetails(Id, PersonId, confirmedAt);
    }

    public void Reject(
        OfficerId officer,
        ChangeOfAddressRejectionReason reason,
        string? notes,
        DateTimeOffset rejectedAt)
    {
        if (Status is not (ChangeOfAddressCaseStatus.Intake
            or ChangeOfAddressCaseStatus.UnderReview
            or ChangeOfAddressCaseStatus.AwaitingPoliceVerification))
            throw new InvalidChangeOfAddressTransitionException(
                $"Cannot perform '{nameof(Reject)}' while the case is in status '{Status}'.");

        DecisionOfficerId = officer;
        RejectionReason = reason;
        DecisionNotes = notes?.Trim();
        Status = ChangeOfAddressCaseStatus.Rejected;
        ClosedAt = rejectedAt;
    }

    public void EnsureIntakeDataEditable(string operation)
    {
        if (Status is not (ChangeOfAddressCaseStatus.Intake
            or ChangeOfAddressCaseStatus.UnderReview
            or ChangeOfAddressCaseStatus.AwaitingPoliceVerification))
            throw new InvalidChangeOfAddressTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
    }

    private void MoveToUnderReviewIfReady()
    {
        if (Status is ChangeOfAddressCaseStatus.AwaitingPoliceVerification) return;

        if (Checklist.NewAddressDeclared &&
            (!Checklist.HousingDocumentRequired || Checklist.HousingDocumentAttached))
            Status = ChangeOfAddressCaseStatus.UnderReview;
    }

    private void EnsureStatus(ChangeOfAddressCaseStatus requiredStatus, string operation)
    {
        if (Status != requiredStatus)
            throw new InvalidChangeOfAddressTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
    }
}