namespace SchaerbeekMunicipality.Application.Auth;

public interface ICurrentOfficer
{
    Guid OfficerId { get; }

    string DisplayName { get; }

    OfficerRole Role { get; }

    bool CanApproveRegistration { get; }

    event Action? Changed;

    void SelectOfficer(Guid officerId);

    void RestoreOfficer(Guid officerId);

    void SetRole(OfficerRole role);

    void Impersonate(Guid officerId, OfficerRole role, string displayName);
}
