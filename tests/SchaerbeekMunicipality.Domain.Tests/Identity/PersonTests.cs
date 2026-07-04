using FluentAssertions;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.Tests.Identity;

public sealed class PersonTests
{
    [Fact]
    public void Update_TrimsAndAppliesIdentityDetails()
    {
        var person = Person.Create(
            new IdentityDetails(" Marie ", " Dupont ", new DateOnly(1990, 5, 15), " Belgian "));

        person.Update(new IdentityDetails("Jean", "Martin", new DateOnly(1985, 1, 1), "French"));

        person.GivenName.Should().Be("Jean");
        person.FamilyName.Should().Be("Martin");
        person.BirthDate.Should().Be(new DateOnly(1985, 1, 1));
        person.Nationality.Should().Be("French");
    }

    [Fact]
    public void Update_WithEmptyGivenName_Throws()
    {
        var person = Person.Create(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1990, 5, 15), "Belgian"));

        var act = () => person.Update(
            new IdentityDetails("", "Dupont", new DateOnly(1990, 5, 15), "Belgian"));

        act.Should().Throw<ArgumentException>();
    }
}
