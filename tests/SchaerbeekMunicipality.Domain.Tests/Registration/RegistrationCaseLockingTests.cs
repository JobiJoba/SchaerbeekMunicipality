using FluentAssertions;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegistrationCaseLockingTests
{
    private static readonly OfficerId OfficerA =
        OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    private static readonly OfficerId OfficerB =
        OfficerId.From(Guid.Parse("44444444-4444-4444-4444-444444444444"));

    private static readonly DateTimeOffset At = new(2026, 7, 5, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Claim_UnassignedCase_AssignsAndLocks()
    {
        var registrationCase = RegistrationCase.Open(VisitReason.FirstRegistration, At);

        var result = registrationCase.Claim(OfficerA, At);

        result.Should().Be(CaseClaimResult.NewlyClaimed);
        registrationCase.AssignedOfficerId.Should().Be(OfficerA);
        registrationCase.LockedByOfficerId.Should().Be(OfficerA);
    }

    [Fact]
    public void Claim_WhenLockedToAnother_Throws()
    {
        var registrationCase = RegistrationCase.Open(VisitReason.FirstRegistration, At);
        registrationCase.Claim(OfficerA, At);

        var act = () => registrationCase.Claim(OfficerB, At);

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void ReleaseLock_OnlyLockHolderCanRelease()
    {
        var registrationCase = RegistrationCase.Open(VisitReason.FirstRegistration, At);
        registrationCase.Claim(OfficerA, At);

        registrationCase.ReleaseLock(OfficerA);

        registrationCase.LockedByOfficerId.Should().BeNull();
        registrationCase.AssignedOfficerId.Should().Be(OfficerA);
    }

    [Fact]
    public void EnsureEditableBy_RequiresActiveLockForOfficer()
    {
        var registrationCase = RegistrationCase.Open(VisitReason.FirstRegistration, At);
        registrationCase.Claim(OfficerA, At);

        var act = () => registrationCase.EnsureEditableBy(OfficerB, "edit");

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }
}
