using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.NationalRegister;
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

    public OfficerId AssignedOfficerId { get; private set; }

    public PersonId? PersonId { get; private set; }

    public ResidenceCategory? ResidenceCategory { get; private set; }

    public ImmigrationDecisionReference? ImmigrationDecision { get; private set; }

    public BelgianAddress? DeclaredAddress { get; private set; }

    public HousingSituation? HousingSituation { get; private set; }

    public DateTimeOffset OpenedAt { get; private set; }

    public RegistrationCaseChecklist Checklist { get; private set; }

    public static RegistrationCase Open(
        VisitReason visitReason,
        OfficerId assignedOfficer,
        DateTimeOffset openedAt)
    {
        return new RegistrationCase
        {
            Id = RegistrationCaseId.New(),
            Status = RegistrationCaseStatus.Intake,
            VisitReason = visitReason,
            AssignedOfficerId = assignedOfficer,
            OpenedAt = openedAt,
            Checklist = RegistrationCaseChecklist.Empty(),
        };
    }

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
}
