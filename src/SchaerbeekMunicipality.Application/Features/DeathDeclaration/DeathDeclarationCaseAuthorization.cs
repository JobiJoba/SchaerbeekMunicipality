using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration;

public sealed class DeathDeclarationCaseAuthorization
{
    public bool CanCreateCase(OfficerRole role)
    {
        return role is OfficerRole.ReceptionOfficer or OfficerRole.PopulationOfficer;
    }

    public bool CanListCases(OfficerRole role)
    {
        return role == OfficerRole.PopulationOfficer;
    }

    public bool CanViewCase(OfficerRole role)
    {
        return role == OfficerRole.PopulationOfficer;
    }

    public bool CanClaimCase(OfficerRole role)
    {
        return role == OfficerRole.PopulationOfficer;
    }

    public bool CanEditCase(OfficerRole role, DeathDeclarationCase deathDeclarationCase, OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               deathDeclarationCase.IsLockedTo(currentOfficerId);
    }

    public bool IsReadOnlyDueToLock(OfficerRole role, DeathDeclarationCase deathDeclarationCase,
        OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               deathDeclarationCase.IsLockedToAnother(currentOfficerId);
    }

    public bool ShouldAutoClaim(DeathDeclarationCase deathDeclarationCase, OfficerId currentOfficerId)
    {
        return deathDeclarationCase.LockedByOfficerId is null &&
               (deathDeclarationCase.AssignedOfficerId is null ||
                deathDeclarationCase.AssignedOfficerId != currentOfficerId);
    }

    public void EnsureCanCreate(ICurrentOfficer officer)
    {
        if (!CanCreateCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to open death declaration cases.");
    }

    public void EnsureCanList(ICurrentOfficer officer)
    {
        if (!CanListCases(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to list death declaration cases.");
    }

    public void EnsureCanView(ICurrentOfficer officer)
    {
        if (!CanViewCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view death declaration cases.");
    }

    public void EnsureCanClaim(ICurrentOfficer officer)
    {
        if (!CanClaimCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to claim death declaration cases.");
    }

    public void EnsureCanEdit(ICurrentOfficer officer, DeathDeclarationCase deathDeclarationCase, string operation)
    {
        if (officer.Role != OfficerRole.PopulationOfficer)
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to modify death declaration cases.");

        var officerId = OfficerId.From(officer.OfficerId);
        deathDeclarationCase.EnsureEditableBy(officerId, operation);
    }
}
