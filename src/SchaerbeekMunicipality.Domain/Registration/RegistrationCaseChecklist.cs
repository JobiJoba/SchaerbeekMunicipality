namespace SchaerbeekMunicipality.Domain.Registration;

public sealed class RegistrationCaseChecklist
{
    private RegistrationCaseChecklist()
    {
    }

    public bool IdentityEstablished { get; private set; }

    public bool LegalResidenceEstablished { get; private set; }

    public bool AddressDeclared { get; private set; }

    public bool AddressConfirmed { get; private set; }

    public bool RegisterDeterminable { get; private set; }

    public bool BirthEvidenceEstablished { get; private set; }

    public bool DuplicateInvestigationResolved { get; private set; }

    public static RegistrationCaseChecklist Empty()
    {
        return new RegistrationCaseChecklist
        {
            DuplicateInvestigationResolved = true
        };
    }

    internal void MarkIdentityEstablished()
    {
        IdentityEstablished = true;
    }

    internal void MarkLegalResidenceEstablished()
    {
        LegalResidenceEstablished = true;
    }

    internal void ClearLegalResidenceEstablished()
    {
        LegalResidenceEstablished = false;
    }

    internal void MarkAddressDeclared()
    {
        AddressDeclared = true;
    }

    internal void MarkAddressConfirmed()
    {
        AddressConfirmed = true;
    }

    internal void ClearAddressConfirmed()
    {
        AddressConfirmed = false;
    }

    internal void MarkRegisterDeterminable()
    {
        RegisterDeterminable = true;
    }

    internal void ClearRegisterDeterminable()
    {
        RegisterDeterminable = false;
    }

    internal void MarkBirthEvidenceEstablished()
    {
        BirthEvidenceEstablished = true;
    }

    internal void ClearBirthEvidenceEstablished()
    {
        BirthEvidenceEstablished = false;
    }

    internal void MarkDuplicateInvestigationResolved()
    {
        DuplicateInvestigationResolved = true;
    }

    internal void ClearDuplicateInvestigationResolved()
    {
        DuplicateInvestigationResolved = false;
    }
}