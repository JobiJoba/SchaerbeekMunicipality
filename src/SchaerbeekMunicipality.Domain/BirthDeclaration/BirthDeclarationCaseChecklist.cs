namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public sealed class BirthDeclarationCaseChecklist
{
    private BirthDeclarationCaseChecklist()
    {
    }

    public bool ChildDetailsRecorded { get; private set; }

    public bool AtLeastOneParentLinked { get; private set; }

    public bool MedicalDeclarationAttached { get; private set; }

    public bool HouseholdEstablished { get; private set; }

    public static BirthDeclarationCaseChecklist Empty()
    {
        return new BirthDeclarationCaseChecklist();
    }

    internal void MarkChildDetailsRecorded()
    {
        ChildDetailsRecorded = true;
    }

    internal void ClearChildDetailsRecorded()
    {
        ChildDetailsRecorded = false;
    }

    internal void MarkAtLeastOneParentLinked()
    {
        AtLeastOneParentLinked = true;
    }

    internal void ClearAtLeastOneParentLinked()
    {
        AtLeastOneParentLinked = false;
    }

    internal void MarkMedicalDeclarationAttached()
    {
        MedicalDeclarationAttached = true;
    }

    internal void ClearMedicalDeclarationAttached()
    {
        MedicalDeclarationAttached = false;
    }

    internal void MarkHouseholdEstablished()
    {
        HouseholdEstablished = true;
    }

    internal void ClearHouseholdEstablished()
    {
        HouseholdEstablished = false;
    }
}