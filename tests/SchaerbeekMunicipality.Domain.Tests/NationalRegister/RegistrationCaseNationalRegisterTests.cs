using FluentAssertions;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Domain.Tests.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.NationalRegister;

public sealed class RegistrationCaseNationalRegisterTests
{
    private static readonly OfficerId DemoOfficer =
        OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));

    private static readonly DateTimeOffset OpenedAt =
        new(2026, 7, 4, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void LinkExistingPerson_CreatesPersonWithRegisterNumbers()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        var registerPerson = NationalRegisterPerson.Create(
            NationalRegisterPersonId.New(),
            "Marie",
            "Leclerc",
            new DateOnly(1975, 1, 1),
            "Belgian",
            BisNumber.Create("75010112345"),
            null);

        var person = registrationCase.LinkExistingPerson(registerPerson);

        person.BisNumber.Should().NotBeNull();
        person.LinkedRegisterRecordId.Should().Be(registerPerson.Id);
        registrationCase.Checklist.IdentityEstablished.Should().BeTrue();
    }

    [Fact]
    public void ConvertBisToNationalRegister_AssignsNumberWhenEligible()
    {
        var person = Person.CreateFromRegisterRecord(
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Marie",
                "Leclerc",
                new DateOnly(1975, 1, 1),
                "Belgian",
                BisNumber.Create("75010112345"),
                null));

        var nationalRegisterNumber = NationalRegisterNumber.GenerateStub(new DateOnly(1975, 1, 1), 42);

        person.ConvertBisToNationalRegister(nationalRegisterNumber);

        person.NationalRegisterNumber.Should().Be(nationalRegisterNumber);
        person.BisNumber.Should().NotBeNull();
    }

    [Fact]
    public void ConvertBisToNationalRegister_WhenAlreadyRegistered_Throws()
    {
        var person = Person.CreateFromRegisterRecord(
            NationalRegisterPerson.Create(
                NationalRegisterPersonId.New(),
                "Jean",
                "Dupont",
                new DateOnly(1985, 6, 12),
                "Belgian",
                null,
                NationalRegisterNumber.GenerateStub(new DateOnly(1985, 6, 12), 1)));

        var act = () => person.ConvertBisToNationalRegister(
            NationalRegisterNumber.GenerateStub(new DateOnly(1985, 6, 12), 2));

        act.Should().Throw<InvalidOperationException>();
    }
}