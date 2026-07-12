using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ClaimChangeOfAddressCase;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ConfirmAddressChange;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.DeclareNewAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ListChangeOfAddressCases;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.OpenChangeOfAddressCase;

namespace SchaerbeekMunicipality.Integration.Tests.Features.ChangeOfAddress;

public sealed class ChangeOfAddressTests
{
    [Fact]
    public async Task ReceptionOfficer_CanOpenCase_ButCannotList()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.ReceptionOfficer);

        var personId = await CreateRegisteredPersonAsync(services);
        var openHandler = services.GetRequiredService<OpenChangeOfAddressCaseHandler>();
        var listHandler = services.GetRequiredService<ListChangeOfAddressCasesHandler>();

        var opened = await openHandler.Handle(new OpenChangeOfAddressCaseRequest(personId), CancellationToken.None);
        opened.CaseId.Should().NotBeEmpty();

        var listAct = () => listHandler.Handle(CancellationToken.None);
        await listAct.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Open_WithoutNationalRegisterNumber_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var db = scope.ServiceProvider.GetRequiredService<MunicipalDbContext>();
        var person = Person.Create(new IdentityDetails("Test", "Person", new DateOnly(1990, 1, 1), "Belgian"));
        await db.Persons.AddAsync(person);
        await db.SaveChangesAsync();

        var handler = scope.ServiceProvider.GetRequiredService<OpenChangeOfAddressCaseHandler>();
        var act = () => handler.Handle(new OpenChangeOfAddressCaseRequest(person.Id.Value), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidChangeOfAddressTransitionException>();
    }

    [Fact]
    public async Task HappyPath_OpenDeclareConfirm_UpdatesPersonDomicile()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var personId = await CreateRegisteredPersonAsync(services);
        var openHandler = services.GetRequiredService<OpenChangeOfAddressCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimChangeOfAddressCaseHandler>();
        var declareHandler = services.GetRequiredService<DeclareNewAddressHandler>();
        var confirmHandler = services.GetRequiredService<ConfirmAddressChangeHandler>();

        var opened = await openHandler.Handle(new OpenChangeOfAddressCaseRequest(personId), CancellationToken.None);
        var caseId = new ChangeOfAddressCaseId(opened.CaseId);
        await claimHandler.Handle(caseId, CancellationToken.None);

        await declareHandler.Handle(
            caseId,
            new DeclareNewAddressRequest(
                "Avenue Rogier",
                "55",
                null,
                "1030",
                "Schaerbeek",
                HousingSituation.Owner,
                new DateOnly(2026, 8, 1)),
            CancellationToken.None);

        var confirmed = await confirmHandler.Handle(caseId, CancellationToken.None);
        confirmed.Status.Should().Be(nameof(ChangeOfAddressCaseStatus.Confirmed));

        var person = await services.GetRequiredService<IPersonRepository>()
            .GetByIdAsync(new PersonId(personId), CancellationToken.None);
        person!.DomicileAddress.Should().NotBeNull();
        person.DomicileAddress!.Street.Should().Be("Avenue Rogier");
    }

    private static async Task<Guid> CreateRegisteredPersonAsync(IServiceProvider services)
    {
        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var jean = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None);
        jean.Should().NotBeNull();

        var personRepo = services.GetRequiredService<IPersonRepository>();
        var existing = await personRepo.GetByRegisterRecordIdAsync(jean!.Id, CancellationToken.None);
        if (existing is not null)
        {
            return existing.Id.Value;
        }

        var person = Person.CreateFromRegisterRecord(jean);
        if (person.NationalRegisterNumber is null && jean.NationalRegisterNumber is { } nr)
        {
            person.AssignNationalRegisterNumber(nr);
        }

        person.UpdateDomicile(BelgianAddress.Create("Rue de la Paix", "1", null, "1030", "Schaerbeek"));
        await personRepo.AddAsync(person, CancellationToken.None);
        await services.GetRequiredService<MunicipalDbContext>().SaveChangesAsync();
        return person.Id.Value;
    }
}
