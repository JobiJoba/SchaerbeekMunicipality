using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

internal static class RegistrationCaseTestData
{
    internal static readonly OfficerId DemoOfficer =
        OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    internal static readonly DateTimeOffset OpenedAt = new(2026, 7, 3, 10, 0, 0, TimeSpan.Zero);

    internal static RegistrationCase OpenClaimedCase(VisitReason visitReason = VisitReason.FirstRegistration)
    {
        var registrationCase = RegistrationCase.Open(visitReason, OpenedAt);
        registrationCase.Claim(DemoOfficer, OpenedAt);
        return registrationCase;
    }
}