using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.CaseManagement;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Domain.Registration;

public sealed class RegistrationCase
{
    private RegistrationCase()
    {
        Checklist = RegistrationCaseChecklist.Empty();
    }

    public RegistrationCaseId Id { get; private set; }

    public RegistrationCaseStatus Status { get; private set; }

    public VisitReason VisitReason { get; private set; }

    public OfficerId? AssignedOfficerId { get; private set; }

    public OfficerId? LockedByOfficerId { get; private set; }

    public DateTimeOffset? LockedAt { get; private set; }

    public PersonId? PersonId { get; private set; }

    public ResidenceCategory? ResidenceCategory { get; private set; }

    public ImmigrationDecisionReference? ImmigrationDecision { get; private set; }

    public BelgianAddress? DeclaredAddress { get; private set; }

    public HousingSituation? HousingSituation { get; private set; }

    public DateTimeOffset OpenedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public RegisterTarget? SelectedRegisterTarget { get; private set; }

    public OfficerId? DecisionOfficerId { get; private set; }

    public RejectionReason? RejectionReason { get; private set; }

    public SuspensionReason? SuspensionReason { get; private set; }

    public string? DecisionNotes { get; private set; }

    public RegistrationCaseStatus? StatusBeforeSuspension { get; private set; }

    public RegistrationCaseChecklist Checklist { get; private set; }

    public DuplicateInvestigationStatus DuplicateInvestigationStatus { get; private set; }

    public static RegistrationCase Open(VisitReason visitReason, DateTimeOffset openedAt)
    {
        return new RegistrationCase
        {
            Id = RegistrationCaseId.New(),
            Status = RegistrationCaseStatus.Intake,
            VisitReason = visitReason,
            OpenedAt = openedAt,
            Checklist = RegistrationCaseChecklist.Empty(),
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
            message => new InvalidRegistrationTransitionException(message));

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
            message => new InvalidRegistrationTransitionException(message));

        LockedByOfficerId = outcome.LockedByOfficerId;
        LockedAt = outcome.LockedAt;
    }

    public void EnsureEditableBy(OfficerId officer, string operation)
    {
        OfficerCaseLocking.EnsureEditableBy(
            LockedByOfficerId,
            officer,
            operation,
            message => new InvalidRegistrationTransitionException(message));
    }

    public bool IsLockedTo(OfficerId officer) =>
        OfficerCaseLocking.IsLockedTo(LockedByOfficerId, officer);

    public bool IsLockedToAnother(OfficerId officer) =>
        OfficerCaseLocking.IsLockedToAnother(LockedByOfficerId, officer);

    public Person RecordIdentity(IdentityDetails identity)
    {
        EnsureStatus(RegistrationCaseStatus.Intake, nameof(RecordIdentity));
        EnsureIdentityNotYetRecorded(nameof(RecordIdentity));

        var person = Person.Create(identity);
        PersonId = person.Id;
        Checklist.MarkIdentityEstablished();

        return person;
    }

    public Person LinkExistingPerson(NationalRegisterPerson registerPerson)
    {
        EnsureStatus(RegistrationCaseStatus.Intake, nameof(LinkExistingPerson));
        EnsureIdentityNotYetRecorded(nameof(LinkExistingPerson));

        var person = Person.CreateFromRegisterRecord(registerPerson);
        PersonId = person.Id;
        Checklist.MarkIdentityEstablished();
        ResolveDuplicateInvestigationAsLinked();

        return person;
    }

    public void CorrectIdentity(Person person, IdentityDetails identity)
    {
        EnsureIntakeDataEditable(nameof(CorrectIdentity));

        if (PersonId is null)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity has not been recorded for this case.");
        }

        if (person.Id != PersonId)
        {
            throw new InvalidRegistrationTransitionException(
                "The person does not belong to this registration case.");
        }

        person.Update(identity);
    }

    public void SetResidenceCategory(ResidenceCategory category)
    {
        EnsureIntakeDataEditable(nameof(SetResidenceCategory));
        EnsureIdentityEstablished("residence information");

        ResidenceCategory = category;
    }

    public void RecordBirthInformation(Person person, BirthInformationDetails details)
    {
        EnsureIntakeDataEditable(nameof(RecordBirthInformation));
        EnsurePersonBelongsToCase(person);

        person.RecordBirthInformation(details);
    }

    public void ApplyBirthEvidenceRule(Person? person, IReadOnlyList<DocumentType> documentTypes)
    {
        if (RegistrationExceptionRules.IsBirthEvidenceComplete(person, documentTypes))
        {
            Checklist.MarkBirthEvidenceEstablished();
        }
        else
        {
            Checklist.ClearBirthEvidenceEstablished();
        }
    }

    public void ApplyDuplicateInvestigationRule(
        Person? person,
        IReadOnlyList<NationalRegisterMatch> matches)
    {
        if (DuplicateInvestigationStatus is DuplicateInvestigationStatus.ResolvedLinked
            or DuplicateInvestigationStatus.ResolvedDistinct)
        {
            Checklist.MarkDuplicateInvestigationResolved();
            return;
        }

        if (RegistrationExceptionRules.RequiresDuplicateInvestigation(person, matches))
        {
            DuplicateInvestigationStatus = DuplicateInvestigationStatus.Open;
            Checklist.ClearDuplicateInvestigationResolved();
            return;
        }

        if (DuplicateInvestigationStatus == DuplicateInvestigationStatus.Open)
        {
            return;
        }

        DuplicateInvestigationStatus = DuplicateInvestigationStatus.None;
        Checklist.MarkDuplicateInvestigationResolved();
    }

    public void ResolveDuplicateAsDistinct(OfficerId officer, string? notes)
    {
        EnsureIntakeDataEditable(nameof(ResolveDuplicateAsDistinct));

        if (DuplicateInvestigationStatus != DuplicateInvestigationStatus.Open)
        {
            throw new InvalidRegistrationTransitionException(
                "Duplicate investigation is not open for this case.");
        }

        DuplicateInvestigationStatus = DuplicateInvestigationStatus.ResolvedDistinct;
        Checklist.MarkDuplicateInvestigationResolved();
    }

    public bool IsIllegalStayDetected(ResidencePolicyResult? policyResult) =>
        RegistrationExceptionRules.IsIllegalStay(this, policyResult);

    public bool IsMarriageRecognitionBlocking(Person? person) =>
        person?.CivilStatus is { } civilStatus && civilStatus.IsMarriageRecognitionBlocking();

    public void RefreshRegisterDeterminability(string? nationality)
    {
        var suggested = RegisterTargetResolver.Suggest(
            ResidenceCategory,
            nationality,
            ImmigrationDecision is not null);
        if (suggested is not null)
        {
            Checklist.MarkRegisterDeterminable();
        }
        else
        {
            Checklist.ClearRegisterDeterminable();
        }
    }

    public void RecordImmigrationDecision(ImmigrationDecisionDetails details)
    {
        EnsureIntakeDataEditable(nameof(RecordImmigrationDecision));
        EnsureIdentityEstablished("residence information");

        ImmigrationDecision = ImmigrationDecisionReference.Create(details);
    }

    public void ApplyResidencePolicyResult(ResidencePolicyResult result)
    {
        if (result.IsValid)
        {
            Checklist.MarkLegalResidenceEstablished();
        }
        else
        {
            Checklist.ClearLegalResidenceEstablished();
        }
    }

    public void DeclareAddress(AddressDetails details)
    {
        EnsureIntakeDataEditable(nameof(DeclareAddress));
        EnsureIdentityEstablished("address information");
        EnsureSchaerbeekDomicile(details);

        DeclaredAddress = BelgianAddress.Create(
            details.Street,
            details.HouseNumber,
            details.Box,
            details.PostalCode,
            details.Municipality);

        Checklist.MarkAddressDeclared();
    }

    public void RecordHousingSituation(HousingSituation situation)
    {
        EnsureIntakeDataEditable(nameof(RecordHousingSituation));
        EnsureIdentityEstablished("housing information");

        if (DeclaredAddress is null)
        {
            throw new InvalidRegistrationTransitionException(
                "Address must be declared before recording housing situation.");
        }

        HousingSituation = situation;
    }

    public void RequestPoliceVerification()
    {
        if (Status is not (RegistrationCaseStatus.Intake or RegistrationCaseStatus.UnderReview))
        {
            throw new InvalidRegistrationTransitionException(
                $"Cannot request police verification while the case is in status '{Status}'.");
        }

        EnsurePoliceVerificationPrerequisites();

        Status = RegistrationCaseStatus.AwaitingPoliceVerification;
    }

    public void ApplyPoliceVerificationResult(PoliceVerificationResult result)
    {
        EnsureStatus(RegistrationCaseStatus.AwaitingPoliceVerification, nameof(ApplyPoliceVerificationResult));

        if (result == PoliceVerificationResult.Confirmed)
        {
            Checklist.MarkAddressConfirmed();
        }
        else
        {
            Checklist.ClearAddressConfirmed();
        }

        Status = RegistrationCaseStatus.UnderReview;
    }

    public bool HasPositivePoliceVerification => Checklist.AddressConfirmed;

    public bool IsReadyForApproval =>
        Status == RegistrationCaseStatus.UnderReview &&
        Checklist.IdentityEstablished &&
        Checklist.LegalResidenceEstablished &&
        Checklist.AddressDeclared &&
        Checklist.AddressConfirmed &&
        Checklist.RegisterDeterminable &&
        Checklist.BirthEvidenceEstablished &&
        Checklist.DuplicateInvestigationResolved &&
        DuplicateInvestigationStatus != DuplicateInvestigationStatus.Open &&
        HasPositivePoliceVerification;

    public void Approve(OfficerId officer, RegisterTarget registerTarget, string? nationality, DateTimeOffset approvedAt)
    {
        EnsureStatus(RegistrationCaseStatus.UnderReview, nameof(Approve));

        if (DuplicateInvestigationStatus == DuplicateInvestigationStatus.Open)
        {
            throw new InvalidRegistrationTransitionException(
                "Cannot approve the case while duplicate identity investigation is open.");
        }

        if (!IsReadyForApproval)
        {
            throw new InvalidRegistrationTransitionException(
                "Cannot approve the case until all review checklist items are satisfied.");
        }

        if (!RegisterTargetResolver.IsAllowed(
                ResidenceCategory,
                nationality,
                registerTarget,
                ImmigrationDecision is not null))
        {
            throw new InvalidRegistrationTransitionException(
                $"Register target '{registerTarget}' is not allowed for this residence category.");
        }

        SelectedRegisterTarget = registerTarget;
        DecisionOfficerId = officer;
        Status = RegistrationCaseStatus.Approved;
        RejectionReason = null;
        SuspensionReason = null;
        DecisionNotes = null;
        StatusBeforeSuspension = null;
        ClosedAt = null;
    }

    public void Reject(
        OfficerId officer,
        RejectionReason reason,
        string? notes,
        DateTimeOffset rejectedAt)
    {
        EnsureStatus(RegistrationCaseStatus.UnderReview, nameof(Reject));

        DecisionOfficerId = officer;
        RejectionReason = reason;
        SuspensionReason = null;
        DecisionNotes = notes?.Trim();
        Status = RegistrationCaseStatus.Rejected;
        StatusBeforeSuspension = null;
        ClosedAt = rejectedAt;
    }

    public void Suspend(
        OfficerId officer,
        SuspensionReason reason,
        string? notes,
        DateTimeOffset suspendedAt)
    {
        if (Status is not (RegistrationCaseStatus.Intake or RegistrationCaseStatus.UnderReview))
        {
            throw new InvalidRegistrationTransitionException(
                $"Cannot suspend the case while it is in status '{Status}'.");
        }

        StatusBeforeSuspension = Status;
        DecisionOfficerId = officer;
        SuspensionReason = reason;
        RejectionReason = null;
        DecisionNotes = notes?.Trim();
        Status = RegistrationCaseStatus.Suspended;
        ClosedAt = null;
    }

    public void ResumeFromSuspension()
    {
        EnsureStatus(RegistrationCaseStatus.Suspended, nameof(ResumeFromSuspension));

        Status = StatusBeforeSuspension ?? RegistrationCaseStatus.Intake;
        StatusBeforeSuspension = null;
        SuspensionReason = null;
        DecisionNotes = null;
        DecisionOfficerId = null;
    }

    public RegistrationConfirmedEventDetails ConfirmRegistration(DateTimeOffset confirmedAt)
    {
        EnsureStatus(RegistrationCaseStatus.Approved, nameof(ConfirmRegistration));

        if (SelectedRegisterTarget is null)
        {
            throw new InvalidRegistrationTransitionException(
                "A register target must be selected before confirming registration.");
        }

        if (PersonId is null)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before confirming registration.");
        }

        Status = RegistrationCaseStatus.Registered;
        ClosedAt = confirmedAt;

        return new RegistrationConfirmedEventDetails(
            Id,
            PersonId.Value,
            SelectedRegisterTarget.Value,
            confirmedAt);
    }

    public void EnsureIntakeDataEditable(string operation)
    {
        if (Status is not (RegistrationCaseStatus.Intake or RegistrationCaseStatus.UnderReview))
        {
            throw new InvalidRegistrationTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
        }
    }

    public void EnsureCanAttachDocuments()
    {
        EnsureIntakeDataEditable(nameof(EnsureCanAttachDocuments));
    }

    private void ResolveDuplicateInvestigationAsLinked()
    {
        DuplicateInvestigationStatus = DuplicateInvestigationStatus.ResolvedLinked;
        Checklist.MarkDuplicateInvestigationResolved();
    }

    private void EnsurePersonBelongsToCase(Person person)
    {
        if (PersonId is null || person.Id != PersonId)
        {
            throw new InvalidRegistrationTransitionException(
                "The person does not belong to this registration case.");
        }

        if (!Checklist.IdentityEstablished)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before birth information can be captured.");
        }
    }

    private void EnsureIdentityEstablished(string stepDescription)
    {
        if (!Checklist.IdentityEstablished)
        {
            throw new InvalidRegistrationTransitionException(
                $"Identity must be recorded before {stepDescription} can be captured.");
        }
    }

    private static void EnsureSchaerbeekDomicile(AddressDetails details)
    {
        if (details.PostalCode.Trim() != SchaerbeekCommune.PostalCode
            || !details.Municipality.Trim().Equals(SchaerbeekCommune.Name, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidRegistrationTransitionException(
                $"Registration at Schaerbeek requires a domicile in {SchaerbeekCommune.PostalCode} {SchaerbeekCommune.Name}.");
        }
    }

    private void EnsureStatus(RegistrationCaseStatus requiredStatus, string operation)
    {
        if (Status != requiredStatus)
        {
            throw new InvalidRegistrationTransitionException(
                $"Cannot perform '{operation}' while the case is in status '{Status}'.");
        }
    }

    private void EnsureIdentityNotYetRecorded(string operation)
    {
        if (PersonId is not null)
        {
            throw new InvalidRegistrationTransitionException(
                $"Identity has already been recorded for this case.");
        }
    }

    private void EnsurePoliceVerificationPrerequisites()
    {
        if (!Checklist.IdentityEstablished)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be established before requesting police verification.");
        }

        if (!Checklist.AddressDeclared)
        {
            throw new InvalidRegistrationTransitionException(
                "Address must be declared before requesting police verification.");
        }

        if (DeclaredAddress is null)
        {
            throw new InvalidRegistrationTransitionException(
                "A declared address is required before requesting police verification.");
        }
    }
}
