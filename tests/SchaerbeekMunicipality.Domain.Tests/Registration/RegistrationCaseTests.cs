using FluentAssertions;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public class RegistrationCaseTests
{
    [Fact]
    public void Create_ReturnsIntakeStatus()
    {
        var registrationCase = RegistrationCase.Create();

        registrationCase.Id.Value.Should().NotBe(Guid.Empty);
        registrationCase.Status.Should().Be(RegistrationCaseStatus.Intake);
    }
}
