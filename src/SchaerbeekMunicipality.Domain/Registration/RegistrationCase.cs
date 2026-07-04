using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;

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

        if (PersonId is not null)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity has already been recorded for this case.");
        }

        var person = Person.Create(identity);
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
        EnsureIdentityEstablished();

        ResidenceCategory = category;
    }

    public void RecordImmigrationDecision(ImmigrationDecisionDetails details)
    {
        EnsureIntakeDataEditable(nameof(RecordImmigrationDecision));
        EnsureIdentityEstablished();

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

    private void EnsureIdentityEstablished()
    {
        if (!Checklist.IdentityEstablished)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before residence information can be captured.");
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
}
