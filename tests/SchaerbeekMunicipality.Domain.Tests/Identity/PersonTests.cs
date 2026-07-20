using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
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

    [Fact]
    public void MarkDeceased_SetsDateOfDeathAndIsDeceased()
    {
        var person = Person.Create(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1940, 5, 15), "Belgian"));

        var deathDate = new DateOnly(2026, 7, 6);
        person.MarkDeceased(deathDate);

        person.IsDeceased.Should().BeTrue();
        person.DateOfDeath.Should().Be(deathDate);
    }

    [Fact]
    public void MarkDeceased_WhenAlreadyDeceased_Throws()
    {
        var person = Person.Create(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1940, 5, 15), "Belgian"));

        person.MarkDeceased(new DateOnly(2026, 7, 6));

        var act = () => person.MarkDeceased(new DateOnly(2026, 7, 7));

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ClearDomicile_RemovesDomicileAddress()
    {
        var person = Person.Create(
            new IdentityDetails("Marie", "Dupont", new DateOnly(1940, 5, 15), "Belgian"));

        person.UpdateDomicile(BelgianAddress.Create("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"));

        person.ClearDomicile();

        person.DomicileAddress.Should().BeNull();
    }
}