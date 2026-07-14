using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments;

public sealed class DocumentRequestCaseAuthorization
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

    public bool CanEditCase(OfficerRole role, DocumentRequestCase documentRequestCase, OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               documentRequestCase.IsLockedTo(currentOfficerId);
    }

    public bool IsReadOnlyDueToLock(OfficerRole role, DocumentRequestCase documentRequestCase,
        OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               documentRequestCase.IsLockedToAnother(currentOfficerId);
    }

    public bool ShouldAutoClaim(DocumentRequestCase documentRequestCase, OfficerId currentOfficerId)
    {
        return documentRequestCase.LockedByOfficerId is null &&
               (documentRequestCase.AssignedOfficerId is null ||
                documentRequestCase.AssignedOfficerId != currentOfficerId);
    }

    public void EnsureCanCreate(ICurrentOfficer officer)
    {
        if (!CanCreateCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to open document request cases.");
    }

    public void EnsureCanList(ICurrentOfficer officer)
    {
        if (!CanListCases(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to list document request cases.");
    }

    public void EnsureCanView(ICurrentOfficer officer)
    {
        if (!CanViewCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view document request cases.");
    }

    public void EnsureCanClaim(ICurrentOfficer officer)
    {
        if (!CanClaimCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to claim document request cases.");
    }

    public void EnsureCanEdit(ICurrentOfficer officer, DocumentRequestCase documentRequestCase, string operation)
    {
        if (officer.Role != OfficerRole.PopulationOfficer)
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to modify document request cases.");

        var officerId = OfficerId.From(officer.OfficerId);
        documentRequestCase.EnsureEditableBy(officerId, operation);
    }
}