namespace SchaerbeekMunicipality.Web.Auth;

public sealed class CurrentOfficer : ICurrentOfficer
{
    public Guid OfficerId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public string DisplayName { get; private set; } = "Marie Dupont (Population)";

    public OfficerRole Role { get; private set; } = OfficerRole.PopulationOfficer;

    public bool CanApproveRegistration => Role == OfficerRole.PopulationOfficer;

    public event Action? Changed;

    public void SetRole(OfficerRole role)
    {
        Role = role;
        DisplayName = role switch
        {
            OfficerRole.ReceptionOfficer => "Jean Martin (Reception)",
            OfficerRole.PopulationOfficer => "Marie Dupont (Population)",
            OfficerRole.PoliceClerk => "Luc Vermeulen (Police)",
            _ => "Demo Officer",
        };

        Changed?.Invoke();
    }
}
