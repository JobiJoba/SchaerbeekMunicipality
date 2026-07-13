using FluentAssertions;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Domain.Tests.NationalRegister;

public sealed class NationalRegisterSearchScorerTests
{
    [Fact]
    public void ScoreMatches_ExactNameAndBirthDate_Returns100Score()
    {
        var criteria = new NationalRegisterSearchCriteria("Amélie", "Bernard", new DateOnly(1992, 3, 20));
        var candidates = new[]
        {
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Amélie",
                "Bernard",
                new DateOnly(1992, 3, 20),
                "French",
                BisNumber.Create("72032054321"),
                null),
        };

        var matches = NationalRegisterSearchScorer.ScoreMatches(criteria, candidates);

        matches.Should().ContainSingle();
        matches[0].MatchScore.Should().Be(NationalRegisterSearchScorer.ExactMatchScore);
    }

    [Fact]
    public void ScoreMatches_SimilarNameSameBirthDate_Returns80Score()
    {
        var criteria = new NationalRegisterSearchCriteria("Jacques", "Dupont", new DateOnly(1985, 6, 12));
        var candidates = new[]
        {
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "J.",
                "Dupont",
                new DateOnly(1985, 6, 12),
                "Belgian",
                BisNumber.Create("75061298765"),
                null),
        };

        var matches = NationalRegisterSearchScorer.ScoreMatches(criteria, candidates);

        matches.Should().ContainSingle();
        matches[0].MatchScore.Should().Be(NationalRegisterSearchScorer.BirthDateAndNameScore);
    }

    [Fact]
    public void ScoreMatches_PartialGivenNameOnly_ReturnsMatches()
    {
        var criteria = new NationalRegisterSearchCriteria("Marie", null, null);
        var candidates = new[]
        {
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Marie",
                "Leclerc",
                new DateOnly(1975, 1, 1),
                "Belgian",
                BisNumber.Create("75010112345"),
                null),
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Sofia",
                "Nguyen",
                new DateOnly(2000, 11, 8),
                "Vietnamese",
                null,
                NationalRegisterNumber.GenerateStub(new DateOnly(2000, 11, 8), 1)),
        };

        var matches = NationalRegisterSearchScorer.ScoreMatches(criteria, candidates);

        matches.Should().ContainSingle();
        matches[0].GivenName.Should().Be("Marie");
        matches[0].MatchScore.Should().Be(NationalRegisterSearchScorer.PartialNameScore);
    }

    [Fact]
    public void ScoreMatches_PartialFamilyName_ReturnsMultipleMatches()
    {
        var criteria = new NationalRegisterSearchCriteria(null, "Dupont", null);
        var candidates = new[]
        {
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Jean",
                "Dupont",
                new DateOnly(1985, 6, 12),
                "Belgian",
                null,
                NationalRegisterNumber.GenerateStub(new DateOnly(1985, 6, 12), 1)),
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "J.",
                "Dupont",
                new DateOnly(1985, 6, 12),
                "Belgian",
                BisNumber.Create("75061298765"),
                null),
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Marie",
                "Leclerc",
                new DateOnly(1975, 1, 1),
                "Belgian",
                BisNumber.Create("75010112345"),
                null),
        };

        var matches = NationalRegisterSearchScorer.ScoreMatches(criteria, candidates);

        matches.Should().HaveCount(2);
        matches.Should().OnlyContain(m => m.FamilyName == "Dupont");
    }

    [Fact]
    public void ScoreMatches_UnrelatedPerson_ReturnsNoMatches()
    {
        var criteria = new NationalRegisterSearchCriteria("Alice", "Martin", new DateOnly(1990, 1, 1));
        var candidates = new[]
        {
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Sofia",
                "Nguyen",
                new DateOnly(2000, 11, 8),
                "Vietnamese",
                null,
                NationalRegisterNumber.GenerateStub(new DateOnly(2000, 11, 8), 1)),
        };

        var matches = NationalRegisterSearchScorer.ScoreMatches(criteria, candidates);

        matches.Should().BeEmpty();
    }

    [Fact]
    public void ScoreMatches_NoCriteria_ReturnsAllCandidatesOrderedByName()
    {
        var criteria = new NationalRegisterSearchCriteria(null, null, null);
        var candidates = new[]
        {
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Sofia",
                "Nguyen",
                new DateOnly(2000, 11, 8),
                "Vietnamese",
                null,
                NationalRegisterNumber.GenerateStub(new DateOnly(2000, 11, 8), 1)),
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Marie",
                "Leclerc",
                new DateOnly(1975, 1, 1),
                "Belgian",
                BisNumber.Create("75010112345"),
                null),
        };

        var matches = NationalRegisterSearchScorer.ScoreMatches(criteria, candidates);

        matches.Should().HaveCount(2);
        matches[0].FamilyName.Should().Be("Leclerc");
        matches[1].FamilyName.Should().Be("Nguyen");
        matches.Should().OnlyContain(m =>
            m.MatchScore == NationalRegisterSearchScorer.BrowseAllScore &&
            m.MatchReason == NationalRegisterSearchScorer.BrowseAllReason);
    }
}
