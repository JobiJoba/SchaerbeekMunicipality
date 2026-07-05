using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration;

public static class OfficerDisplayNames
{
    public static Guid SecondaryPopulationOfficerId => CurrentOfficer.SecondaryPopulationOfficerId;

    public static string For(Guid? officerId) =>
        officerId switch
        {
            null => "Unassigned",
            _ => DemoOfficers.Find(officerId.Value).DisplayName,
        };

    public static string LockStatus(Guid? assignedOfficerId, Guid? lockedByOfficerId)
    {
        if (lockedByOfficerId is null)
        {
            return assignedOfficerId is null ? "Unassigned" : $"Assigned — {For(assignedOfficerId)}";
        }

        return $"Locked — {For(lockedByOfficerId)}";
    }
}
