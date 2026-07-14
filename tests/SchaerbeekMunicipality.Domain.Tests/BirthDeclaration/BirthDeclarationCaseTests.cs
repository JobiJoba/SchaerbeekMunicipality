using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.Tests.BirthDeclaration;

public sealed class BirthDeclarationCaseTests
{
    [Fact]
    public void ConfirmDeclaration_WithoutMedicalDeclaration_Throws()
    {
        var birthDeclarationCase = BirthDeclarationCaseTestData.OpenClaimedCase();
        var personId = PersonId.New();

        birthDeclarationCase.RecordChildDetails(
            new NewbornDetails("Amélie", "Dupont", NewbornSex.Female, BirthDeclarationCaseTestData.Today.AddDays(-1),
                null, "Brussels"),
            BirthDeclarationCaseTestData.Today);

        birthDeclarationCase.LinkParent(personId, ParentRole.Mother);
        birthDeclarationCase.SetHousehold(
            BelgianAddress.Create("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"));

        var act = () => birthDeclarationCase.ConfirmDeclaration(
            PersonId.New(),
            "99010712345",
            BirthDeclarationCaseTestData.OpenedAt.AddDays(1));

        act.Should().Throw<InvalidBirthDeclarationTransitionException>()
            .WithMessage("*checklist*");
    }

    [Fact]
    public void ConfirmDeclaration_WithoutParentLink_Throws()
    {
        var birthDeclarationCase = BirthDeclarationCaseTestData.OpenClaimedCase();

        birthDeclarationCase.RecordChildDetails(
            new NewbornDetails("Amélie", "Dupont", NewbornSex.Female, BirthDeclarationCaseTestData.Today.AddDays(-1),
                null, "Brussels"),
            BirthDeclarationCaseTestData.Today);

        birthDeclarationCase.AttachMedicalDeclaration(AdministrativeDocumentId.New());
        birthDeclarationCase.SetHousehold(
            BelgianAddress.Create("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"));

        var act = () => birthDeclarationCase.ConfirmDeclaration(
            PersonId.New(),
            "99010712345",
            BirthDeclarationCaseTestData.OpenedAt.AddDays(1));

        act.Should().Throw<InvalidBirthDeclarationTransitionException>()
            .WithMessage("*checklist*");
    }

    [Fact]
    public void RecordChildDetails_WithFutureDateOfBirth_Throws()
    {
        var birthDeclarationCase = BirthDeclarationCaseTestData.OpenClaimedCase();

        var act = () => birthDeclarationCase.RecordChildDetails(
            new NewbornDetails("Amélie", "Dupont", NewbornSex.Female, BirthDeclarationCaseTestData.Today.AddDays(1),
                null, "Brussels"),
            BirthDeclarationCaseTestData.Today);

        act.Should().Throw<InvalidBirthDeclarationTransitionException>()
            .WithMessage("*future*");
    }

    [Fact]
    public void ConfirmDeclaration_WhenReady_TransitionsToConfirmed()
    {
        var birthDeclarationCase = BirthDeclarationCaseTestData.CaseReadyForConfirmation();
        var childPersonId = PersonId.New();

        birthDeclarationCase.ConfirmDeclaration(
            childPersonId,
            "99010712345",
            BirthDeclarationCaseTestData.OpenedAt.AddDays(1));

        birthDeclarationCase.Status.Should().Be(BirthDeclarationCaseStatus.Confirmed);
        birthDeclarationCase.ChildNationalRegisterNumber.Should().Be("99010712345");
    }
}