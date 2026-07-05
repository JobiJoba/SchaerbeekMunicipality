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
    public void Suggest_ReturnsExpectedTarget(
        ResidenceCategory category,
        string nationality,
        RegisterTarget expected)
    {
        RegisterTargetResolver.Suggest(category, nationality).Should().Be(expected);
    }
}
