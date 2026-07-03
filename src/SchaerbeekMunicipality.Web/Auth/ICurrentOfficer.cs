namespace SchaerbeekMunicipality.Web.Auth;

public interface ICurrentOfficer
{
    Guid OfficerId { get; }

    string DisplayName { get; }

    OfficerRole Role { get; }

    bool CanApproveRegistration { get; }

    event Action? Changed;

    void SetRole(OfficerRole role);
}
