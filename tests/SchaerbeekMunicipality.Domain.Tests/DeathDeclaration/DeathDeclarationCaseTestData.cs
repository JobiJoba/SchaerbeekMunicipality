using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.DeathDeclaration;

internal static class DeathDeclarationCaseTestData
{
    internal static readonly OfficerId DemoOfficer =
        OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    internal static readonly DateTimeOffset OpenedAt =
        new(2026, 7, 6, 10, 0, 0, TimeSpan.Zero);

    internal static readonly DateOnly Today = new(2026, 7, 7);

    internal static DeathDeclarationCase OpenClaimedCase()
    {
        var deathDeclarationCase = DeathDeclarationCase.Open(PersonId.New(), OpenedAt);
        deathDeclarationCase.Claim(DemoOfficer, OpenedAt);
        return deathDeclarationCase;
    }

    internal static DeathFacts SampleFacts()
    {
        return new DeathFacts(Today.AddDays(-1), "CHU Saint-Pierre", false, InformantRelationship.Spouse);
    }

    internal static DeathDeclarationCase CaseReadyForConfirmation()
    {
        var deathDeclarationCase = OpenClaimedCase();

        deathDeclarationCase.RecordDeathFacts(SampleFacts(), Today);
        deathDeclarationCase.AttachDeathAct(AdministrativeDocumentId.New());
        deathDeclarationCase.ReviewHousehold(OpenedAt.AddHours(1));

        return deathDeclarationCase;
    }
}
