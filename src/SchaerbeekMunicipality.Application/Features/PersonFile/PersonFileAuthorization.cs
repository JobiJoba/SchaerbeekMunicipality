using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.PersonFile;

public sealed class PersonFileAuthorization
{
    public bool CanView(OfficerRole role)
    {
        return role == OfficerRole.PopulationOfficer;
    }

    public void EnsureCanView(ICurrentOfficer officer)
    {
        if (!CanView(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view person files.");
    }
}