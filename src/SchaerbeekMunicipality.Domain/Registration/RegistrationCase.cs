using SchaerbeekMunicipality.Domain.Identity;

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

    public void EnsureCanAttachDocuments()
    {
        if (Status is not (RegistrationCaseStatus.Intake or RegistrationCaseStatus.UnderReview))
        {
            throw new InvalidRegistrationTransitionException(
                $"Documents cannot be attached while the case is in status '{Status}'.");
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
