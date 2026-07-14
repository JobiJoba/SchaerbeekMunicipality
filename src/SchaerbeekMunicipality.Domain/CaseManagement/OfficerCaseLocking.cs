using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.CaseManagement;

public static class OfficerCaseLocking
{
    public static ClaimOutcome Claim(
        OfficerId? assignedOfficerId,
        OfficerId? lockedByOfficerId,
        DateTimeOffset? lockedAt,
        OfficerId officer,
        DateTimeOffset at,
        Func<string, Exception> createException)
    {
        if (lockedByOfficerId is { } locked && locked != officer)
            throw createException("This case is locked to another officer.");

        if (lockedByOfficerId == officer)
            return new ClaimOutcome(assignedOfficerId, lockedByOfficerId, lockedAt, CaseClaimResult.AlreadyHeld);

        var hadAssignee = assignedOfficerId is not null;
        return new ClaimOutcome(
            officer,
            officer,
            at,
            hadAssignee ? CaseClaimResult.Reclaimed : CaseClaimResult.NewlyClaimed);
    }

    public static ReleaseOutcome ReleaseLock(
        OfficerId? lockedByOfficerId,
        OfficerId officer,
        Func<string, Exception> createException)
    {
        if (lockedByOfficerId != officer) throw createException("Only the officer holding the lock can release it.");

        return new ReleaseOutcome(null, null);
    }

    public static void EnsureEditableBy(
        OfficerId? lockedByOfficerId,
        OfficerId officer,
        string operation,
        Func<string, Exception> createException)
    {
        if (lockedByOfficerId is null)
            throw createException($"Cannot perform '{operation}' before the case is claimed.");

        if (lockedByOfficerId != officer)
            throw createException($"Cannot perform '{operation}' while the case is locked to another officer.");
    }

    public static bool IsLockedTo(OfficerId? lockedByOfficerId, OfficerId officer)
    {
        return lockedByOfficerId == officer;
    }

    public static bool IsLockedToAnother(OfficerId? lockedByOfficerId, OfficerId officer)
    {
        return lockedByOfficerId is { } locked && locked != officer;
    }

    public readonly record struct ClaimOutcome(
        OfficerId? AssignedOfficerId,
        OfficerId? LockedByOfficerId,
        DateTimeOffset? LockedAt,
        CaseClaimResult Result);

    public readonly record struct ReleaseOutcome(
        OfficerId? LockedByOfficerId,
        DateTimeOffset? LockedAt);
}