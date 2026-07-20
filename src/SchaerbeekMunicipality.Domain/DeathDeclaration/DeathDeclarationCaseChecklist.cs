namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public sealed class DeathDeclarationCaseChecklist
{
    private DeathDeclarationCaseChecklist()
    {
    }

    public bool PersonIdentified { get; private set; }

    public bool DeathFactsRecorded { get; private set; }

    public bool DeathActAttached { get; private set; }

    public bool HouseholdReviewed { get; private set; }

    public static DeathDeclarationCaseChecklist Empty()
    {
        return new DeathDeclarationCaseChecklist();
    }

    internal void MarkPersonIdentified()
    {
        PersonIdentified = true;
    }

    internal void MarkDeathFactsRecorded()
    {
        DeathFactsRecorded = true;
    }

    internal void MarkDeathActAttached()
    {
        DeathActAttached = true;
    }

    internal void ClearDeathActAttached()
    {
        DeathActAttached = false;
    }

    internal void MarkHouseholdReviewed()
    {
        HouseholdReviewed = true;
    }
}
