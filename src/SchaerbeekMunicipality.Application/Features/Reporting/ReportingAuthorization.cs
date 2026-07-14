using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Reporting;

public sealed class ReportingAuthorization
{
    public bool CanViewReports(OfficerRole role)
    {
        return role is OfficerRole.PopulationOfficer or OfficerRole.BackOfficeOfficer;
    }

    public void EnsureCanViewReports(ICurrentOfficer officer)
    {
        if (!CanViewReports(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view municipality reports.");
    }
}