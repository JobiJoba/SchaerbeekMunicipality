namespace SchaerbeekMunicipality.Web.Auth;

public sealed record DemoOfficer(Guid Id, OfficerRole Role, string DisplayName);

public static class DemoOfficers
{
    public static readonly DemoOfficer Jean = new(
        CurrentOfficer.ReceptionOfficerId,
        OfficerRole.ReceptionOfficer,
        "Jean Martin (Reception)");

    public static readonly DemoOfficer Marie = new(
        CurrentOfficer.PopulationOfficerId,
        OfficerRole.PopulationOfficer,
        "Marie Dupont (Population)");

    public static readonly DemoOfficer Anne = new(
        CurrentOfficer.SecondaryPopulationOfficerId,
        OfficerRole.PopulationOfficer,
        "Anne Leroy (Population)");

    public static readonly DemoOfficer Luc = new(
        CurrentOfficer.PoliceClerkId,
        OfficerRole.PoliceClerk,
        "Luc Vermeulen (Police)");

    public static IReadOnlyList<DemoOfficer> All { get; } = [Jean, Marie, Anne, Luc];

    public static DemoOfficer Find(Guid officerId) =>
        All.FirstOrDefault(o => o.Id == officerId) ?? Marie;
}
