using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Household;

public sealed class Household
{
    private readonly List<HouseholdMember> _members = [];

    private Household()
    {
    }

    public HouseholdId Id { get; private set; }

    public RegistrationCaseId RegistrationCaseId { get; private set; }

    public IReadOnlyList<HouseholdMember> Members => _members.AsReadOnly();

    public static Household Create(RegistrationCaseId registrationCaseId)
    {
        return new Household
        {
            Id = HouseholdId.New(),
            RegistrationCaseId = registrationCaseId
        };
    }

    public void SetComposition(IReadOnlyList<HouseholdMemberDetails> members)
    {
        _members.Clear();

        foreach (var member in members)
        {
            var householdMember = HouseholdMember.Create(member);
            householdMember.AttachToHousehold(Id);
            _members.Add(householdMember);
        }
    }
}