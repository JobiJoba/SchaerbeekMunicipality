using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration;

public sealed class RegistrationCaseAuthorization
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

    public bool CanViewReviewDashboard(OfficerRole role)
    {
        return role == OfficerRole.PopulationOfficer;
    }

    public bool CanRecordPoliceResult(OfficerRole role)
    {
        return role == OfficerRole.PoliceClerk;
    }

    public bool CanClaimCase(OfficerRole role)
    {
        return role == OfficerRole.PopulationOfficer;
    }

    public bool CanEditCase(OfficerRole role, RegistrationCase registrationCase, OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               registrationCase.IsLockedTo(currentOfficerId);
    }

    public bool IsReadOnlyDueToLock(OfficerRole role, RegistrationCase registrationCase, OfficerId currentOfficerId)
    {
        return role == OfficerRole.PopulationOfficer &&
               registrationCase.IsLockedToAnother(currentOfficerId);
    }

    public bool ShouldAutoClaim(RegistrationCase registrationCase, OfficerId currentOfficerId)
    {
        return registrationCase.LockedByOfficerId is null &&
               (registrationCase.AssignedOfficerId is null ||
                registrationCase.AssignedOfficerId != currentOfficerId);
    }

    public void EnsureCanCreate(ICurrentOfficer officer)
    {
        if (!CanCreateCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to open registration cases.");
    }

    public void EnsureCanList(ICurrentOfficer officer)
    {
        if (!CanListCases(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to list registration cases.");
    }

    public void EnsureCanView(ICurrentOfficer officer)
    {
        if (!CanViewCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view registration cases.");
    }

    public void EnsureCanViewReviewDashboard(ICurrentOfficer officer)
    {
        if (!CanViewReviewDashboard(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to view the review dashboard.");
    }

    public void EnsureCanClaim(ICurrentOfficer officer)
    {
        if (!CanClaimCase(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to claim registration cases.");
    }

    public void EnsureCanEdit(ICurrentOfficer officer, RegistrationCase registrationCase, string operation)
    {
        if (officer.Role != OfficerRole.PopulationOfficer)
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to modify registration cases.");

        var officerId = OfficerId.From(officer.OfficerId);
        registrationCase.EnsureEditableBy(officerId, operation);
    }

    public void EnsureCanRecordPoliceResult(ICurrentOfficer officer)
    {
        if (!CanRecordPoliceResult(officer.Role))
            throw new UnauthorizedAccessException(
                $"Role '{officer.Role}' is not allowed to record police verification results.");
    }
}