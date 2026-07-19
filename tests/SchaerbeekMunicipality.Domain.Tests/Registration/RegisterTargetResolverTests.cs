using FluentAssertions;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegisterTargetResolverTests
{
    [Theory]
    [InlineData(ResidenceCategory.EuCitizen, "Belgian", RegisterTarget.PopulationRegister)]
    [InlineData(ResidenceCategory.EuCitizen, "French", RegisterTarget.ForeignersRegister)]
    [InlineData(ResidenceCategory.NonEuWorker, "Moroccan", RegisterTarget.ForeignersRegister)]
    [InlineData(ResidenceCategory.Student, "Chinese", RegisterTarget.ForeignersRegister)]
    [InlineData(ResidenceCategory.Refugee, "Syrian", RegisterTarget.WaitingRegister)]
    [InlineData(ResidenceCategory.Diplomat, "American", RegisterTarget.SpecialRegister)]
    public void Suggest_ReturnsExpectedTarget(
        ResidenceCategory category,
        string nationality,
        RegisterTarget expected)
    {
        var hasDecision = category == ResidenceCategory.Refugee;
        RegisterTargetResolver.Suggest(category, nationality, hasDecision).Should().Be(expected);
    }
}