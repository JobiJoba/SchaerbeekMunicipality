using FluentAssertions;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Domain.Tests.DeathDeclaration;

public sealed class DeathDeclarationCaseTests
{
    [Fact]
    public void ConfirmRadiation_WithoutDeathAct_Throws()
    {
        var deathDeclarationCase = DeathDeclarationCaseTestData.OpenClaimedCase();

        deathDeclarationCase.RecordDeathFacts(DeathDeclarationCaseTestData.SampleFacts(), DeathDeclarationCaseTestData.Today);
        deathDeclarationCase.ReviewHousehold(DeathDeclarationCaseTestData.OpenedAt.AddHours(1));

        var act = () => deathDeclarationCase.ConfirmRadiation(DeathDeclarationCaseTestData.OpenedAt.AddDays(1));

        act.Should().Throw<InvalidDeathDeclarationTransitionException>()
            .WithMessage("*checklist*");
    }

    [Fact]
    public void ConfirmRadiation_WithoutHouseholdReview_Throws()
    {
        var deathDeclarationCase = DeathDeclarationCaseTestData.OpenClaimedCase();

        deathDeclarationCase.RecordDeathFacts(DeathDeclarationCaseTestData.SampleFacts(), DeathDeclarationCaseTestData.Today);
        deathDeclarationCase.AttachDeathAct(AdministrativeDocumentId.New());

        var act = () => deathDeclarationCase.ConfirmRadiation(DeathDeclarationCaseTestData.OpenedAt.AddDays(1));

        act.Should().Throw<InvalidDeathDeclarationTransitionException>()
            .WithMessage("*checklist*");
    }

    [Fact]
    public void RecordDeathFacts_WithFutureDate_Throws()
    {
        var deathDeclarationCase = DeathDeclarationCaseTestData.OpenClaimedCase();

        var facts = new DeathFacts(
            DeathDeclarationCaseTestData.Today.AddDays(1),
            "CHU Saint-Pierre",
            false,
            InformantRelationship.Spouse);

        var act = () => deathDeclarationCase.RecordDeathFacts(facts, DeathDeclarationCaseTestData.Today);

        act.Should().Throw<InvalidDeathDeclarationTransitionException>()
            .WithMessage("*future*");
    }

    [Fact]
    public void RecordDeathFacts_AutoTransitionsToUnderReview()
    {
        var deathDeclarationCase = DeathDeclarationCaseTestData.OpenClaimedCase();

        deathDeclarationCase.RecordDeathFacts(DeathDeclarationCaseTestData.SampleFacts(), DeathDeclarationCaseTestData.Today);

        deathDeclarationCase.Status.Should().Be(DeathDeclarationCaseStatus.UnderReview);
    }

    [Fact]
    public void ConfirmRadiation_WhenReady_TransitionsToConfirmed()
    {
        var deathDeclarationCase = DeathDeclarationCaseTestData.CaseReadyForConfirmation();

        var details = deathDeclarationCase.ConfirmRadiation(DeathDeclarationCaseTestData.OpenedAt.AddDays(1));

        deathDeclarationCase.Status.Should().Be(DeathDeclarationCaseStatus.Confirmed);
        details.PersonId.Should().Be(deathDeclarationCase.PersonId);
        details.DeathDate.Should().Be(deathDeclarationCase.DeathDate!.Value);
    }
}
