using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress;

public sealed class ChangeOfAddressCaseAuthorization
{
    public bool CanCreateCase(OfficerRole role) =>
        role is OfficerRole.ReceptionOfficer or OfficerRole.PopulationOfficer;

    public bool CanListCases(OfficerRole role) =>
        role == OfficerRole.PopulationOfficer;

    public bool CanViewCase(OfficerRole role) =>
        role == OfficerRole.PopulationOfficer;

    public bool CanClaimCase(OfficerRole role) =>
        role == OfficerRole.PopulationOfficer;

    public bool CanEditCase(OfficerRole role, ChangeOfAddressCase changeOfAddressCase, OfficerId currentOfficerId) =>
        role == OfficerRole.PopulationOfficer &&
        changeOfAddressCase.IsLockedTo(currentOfficerId);

    public bool IsReadOnlyDueToLock(OfficerRole role, ChangeOfAddressCase changeOfAddressCase, OfficerId currentOfficerId) =>
        role == OfficerRole.PopulationOfficer &&
        changeOfAddressCase.IsLockedToAnother(currentOfficerId);

    public bool ShouldAutoClaim(ChangeOfAddressCase changeOfAddressCase, OfficerId currentOfficerId) =>
        changeOfAddressCase.LockedByOfficerId is null &&
        (changeOfAddressCase.AssignedOfficerId is null ||
         changeOfAddressCase.AssignedOfficerId != currentOfficerId);

    public void EnsureCanCreate(ICurrentOfficer officer)
    {
        if (!CanCreateCase(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to open change of address cases.");
        }
    }

    public void EnsureCanList(ICurrentOfficer officer)
    {
        if (!CanListCases(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to list change of address cases.");
        }
    }

    public void EnsureCanView(ICurrentOfficer officer)
    {
        if (!CanViewCase(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view change of address cases.");
        }
    }

    public void EnsureCanClaim(ICurrentOfficer officer)
    {
        if (!CanClaimCase(officer.Role))
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to claim change of address cases.");
        }
    }

    public void EnsureCanEdit(ICurrentOfficer officer, ChangeOfAddressCase changeOfAddressCase, string operation)
    {
        if (officer.Role != OfficerRole.PopulationOfficer)
        {
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to modify change of address cases.");
        }

        var officerId = OfficerId.From(officer.OfficerId);
        changeOfAddressCase.EnsureEditableBy(officerId, operation);
    }
}
