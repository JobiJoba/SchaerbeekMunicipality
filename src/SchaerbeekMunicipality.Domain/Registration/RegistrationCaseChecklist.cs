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

    public static RegistrationCaseChecklist Empty() => new();

    internal void MarkIdentityEstablished() => IdentityEstablished = true;

    internal void MarkLegalResidenceEstablished() => LegalResidenceEstablished = true;

    internal void ClearLegalResidenceEstablished() => LegalResidenceEstablished = false;

    internal void MarkAddressDeclared() => AddressDeclared = true;
}
