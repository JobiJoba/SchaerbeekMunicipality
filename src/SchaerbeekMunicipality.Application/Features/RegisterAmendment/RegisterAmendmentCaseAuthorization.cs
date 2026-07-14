using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment;

public sealed class RegisterAmendmentCaseAuthorization
{
    public bool CanCreateCase(OfficerRole role)
    {
        return role == OfficerRole.PopulationOfficer;
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

    public bool CanEditCase(OfficerRole role, RegisterAmendmentCase amendmentCase, OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               amendmentCase.IsLockedTo(currentOfficerId);
    }

    public bool IsReadOnlyDueToLock(OfficerRole role, RegisterAmendmentCase amendmentCase, OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               amendmentCase.IsLockedToAnother(currentOfficerId);
    }

    public bool ShouldAutoClaim(RegisterAmendmentCase amendmentCase, OfficerId currentOfficerId)
    {
        return amendmentCase.LockedByOfficerId is null &&
               (amendmentCase.AssignedOfficerId is null ||
                amendmentCase.AssignedOfficerId != currentOfficerId);
    }

    public void EnsureCanCreate(ICurrentOfficer officer)
    {
        if (!CanCreateCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to open register amendment cases.");
    }

    public void EnsureCanList(ICurrentOfficer officer)
    {
        if (!CanListCases(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to list register amendment cases.");
    }

    public void EnsureCanView(ICurrentOfficer officer)
    {
        if (!CanViewCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view register amendment cases.");
    }

    public void EnsureCanClaim(ICurrentOfficer officer)
    {
        if (!CanClaimCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to claim register amendment cases.");
    }

    public void EnsureCanEdit(ICurrentOfficer officer, RegisterAmendmentCase amendmentCase, string operation)
    {
        if (officer.Role != OfficerRole.PopulationOfficer)
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to modify register amendment cases.");

        var officerId = OfficerId.From(officer.OfficerId);
        amendmentCase.EnsureEditableBy(officerId, operation);
    }

    public void EnsureCanApprove(ICurrentOfficer officer)
    {
        if (!officer.CanApproveRegistration)
            throw new UnauthorizedAccessException(
                "Only population officers can approve or reject register amendment cases.");
    }
}
