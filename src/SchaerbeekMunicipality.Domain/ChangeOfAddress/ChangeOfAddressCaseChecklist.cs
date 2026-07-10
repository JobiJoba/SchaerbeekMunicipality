namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public sealed class ChangeOfAddressCaseChecklist
{
    private ChangeOfAddressCaseChecklist()
    {
    }

    public bool PersonIdentified { get; private set; }

    public bool NewAddressDeclared { get; private set; }

    public bool HousingDocumentAttached { get; private set; }

    public bool HousingDocumentRequired { get; private set; }

    public bool PoliceVerificationRequested { get; private set; }

    public bool PoliceVerificationPositive { get; private set; }

    public static ChangeOfAddressCaseChecklist Empty() => new();

    internal void MarkPersonIdentified() => PersonIdentified = true;

    internal void MarkNewAddressDeclared() => NewAddressDeclared = true;

    internal void MarkHousingDocumentAttached() => HousingDocumentAttached = true;

    internal void ClearHousingDocumentAttached() => HousingDocumentAttached = false;

    internal void SetHousingDocumentRequired(bool required) => HousingDocumentRequired = required;

    internal void MarkPoliceVerificationRequested() => PoliceVerificationRequested = true;

    internal void MarkPoliceVerificationPositive() => PoliceVerificationPositive = true;

    internal void ClearPoliceVerificationPositive() => PoliceVerificationPositive = false;
}
