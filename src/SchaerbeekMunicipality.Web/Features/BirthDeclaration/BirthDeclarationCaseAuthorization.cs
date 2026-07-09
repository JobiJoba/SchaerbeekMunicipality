using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration;

public sealed class BirthDeclarationCaseAuthorization
{
    public bool CanCreateCase(OfficerRole role) =>
        role is OfficerRole.ReceptionOfficer or OfficerRole.PopulationOfficer;

    public bool CanListCases(OfficerRole role) =>
        role == OfficerRole.PopulationOfficer;

    public bool CanViewCase(OfficerRole role) =>
        role == OfficerRole.PopulationOfficer;

    public bool CanClaimCase(OfficerRole role) =>
        role == OfficerRole.PopulationOfficer;

    public bool CanEditCase(OfficerRole role, BirthDeclarationCase birthDeclarationCase, OfficerId currentOfficerId) =>
        role == OfficerRole.PopulationOfficer &&
        birthDeclarationCase.IsLockedTo(currentOfficerId);

    public bool IsReadOnlyDueToLock(OfficerRole role, BirthDeclarationCase birthDeclarationCase, OfficerId currentOfficerId) =>
        role == OfficerRole.PopulationOfficer &&
        birthDeclarationCase.IsLockedToAnother(currentOfficerId);

    public bool ShouldAutoClaim(BirthDeclarationCase birthDeclarationCase, OfficerId currentOfficerId) =>
        birthDeclarationCase.LockedByOfficerId is null &&
        (birthDeclarationCase.AssignedOfficerId is null ||
         birthDeclarationCase.AssignedOfficerId != currentOfficerId);

    public void EnsureCanCreate(ICurrentOfficer officer)
    {
        if (!CanCreateCase(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to open birth declaration cases.");
        }
    }

    public void EnsureCanList(ICurrentOfficer officer)
    {
        if (!CanListCases(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to list birth declaration cases.");
        }
    }

    public void EnsureCanView(ICurrentOfficer officer)
    {
        if (!CanViewCase(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view birth declaration cases.");
        }
    }

    public void EnsureCanClaim(ICurrentOfficer officer)
    {
        if (!CanClaimCase(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to claim birth declaration cases.");
        }
    }

    public void EnsureCanEdit(ICurrentOfficer officer, BirthDeclarationCase birthDeclarationCase, string operation)
    {
        if (officer.Role != OfficerRole.PopulationOfficer)
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to modify birth declaration cases.");
        }

        var officerId = OfficerId.From(officer.OfficerId);
        birthDeclarationCase.EnsureEditableBy(officerId, operation);
    }
}
