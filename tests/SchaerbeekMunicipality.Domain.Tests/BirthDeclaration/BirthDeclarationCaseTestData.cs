using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.BirthDeclaration;

internal static class BirthDeclarationCaseTestData
{
    internal static readonly OfficerId DemoOfficer =
        OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    internal static readonly DateTimeOffset OpenedAt =
        new(2026, 7, 6, 10, 0, 0, TimeSpan.Zero);

    internal static readonly DateOnly Today = new(2026, 7, 7);

    internal static BirthDeclarationCase OpenClaimedCase()
    {
        var birthDeclarationCase = BirthDeclarationCase.Open(OpenedAt);
        birthDeclarationCase.Claim(DemoOfficer, OpenedAt);
        return birthDeclarationCase;
    }

    internal static BirthDeclarationCase CaseReadyForConfirmation()
    {
        var birthDeclarationCase = OpenClaimedCase();
        var personId = PersonId.New();

        birthDeclarationCase.RecordChildDetails(
            new NewbornDetails("Amélie", "Dupont", NewbornSex.Female, Today.AddDays(-1), null, "CHU Saint-Pierre"),
            Today);

        birthDeclarationCase.LinkParent(personId, ParentRole.Mother);
        birthDeclarationCase.AttachMedicalDeclaration(AdministrativeDocumentId.New());
        birthDeclarationCase.SetHousehold(
            BelgianAddress.Create("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"));

        return birthDeclarationCase;
    }
}