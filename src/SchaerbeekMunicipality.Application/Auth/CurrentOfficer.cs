namespace SchaerbeekMunicipality.Application.Auth;

public sealed class CurrentOfficer : ICurrentOfficer
{
    public static readonly Guid ReceptionOfficerId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid PopulationOfficerId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid SecondaryPopulationOfficerId =
        Guid.Parse("44444444-4444-4444-4444-444444444444");

    public static readonly Guid PoliceClerkId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    public Guid OfficerId { get; private set; } = PopulationOfficerId;

    public string DisplayName { get; private set; } = DemoOfficers.Marie.DisplayName;

    public OfficerRole Role { get; private set; } = OfficerRole.PopulationOfficer;

    public bool CanApproveRegistration => Role == OfficerRole.PopulationOfficer;

    public event Action? Changed;

    public void SelectOfficer(Guid officerId)
    {
        Apply(DemoOfficers.Find(officerId), notify: true);
    }

    public void RestoreOfficer(Guid officerId)
    {
        Apply(DemoOfficers.Find(officerId), notify: false);
    }

    public void SetRole(OfficerRole role)
    {
        var officer = role switch
        {
            OfficerRole.ReceptionOfficer => DemoOfficers.Jean,
            OfficerRole.PopulationOfficer => DemoOfficers.Marie,
            OfficerRole.PoliceClerk => DemoOfficers.Luc,
            _ => DemoOfficers.Marie,
        };

        Apply(officer, notify: true);
    }

    public void Impersonate(Guid officerId, OfficerRole role, string displayName)
    {
        OfficerId = officerId;
        Role = role;
        DisplayName = displayName;
        Changed?.Invoke();
    }

    private void Apply(DemoOfficer officer, bool notify)
    {
        OfficerId = officer.Id;
        Role = officer.Role;
        DisplayName = officer.DisplayName;

        if (notify)
        {
            Changed?.Invoke();
        }
    }
}
