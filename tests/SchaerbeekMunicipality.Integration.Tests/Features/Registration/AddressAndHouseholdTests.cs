using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.RecordCivilStatus;
using SchaerbeekMunicipality.Application.Features.Registration.RecordHouseholdComposition;
using SchaerbeekMunicipality.Application.Features.Registration.RecordHousingSituation;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class AddressAndHouseholdTests
{
    private static async Task<RegistrationCaseId> OpenAndRecordIdentityAsync(IServiceProvider services)
    {
        var openHandler = services.GetRequiredService<OpenRegistrationCaseHandler>();
        var recordHandler = services.GetRequiredService<RecordIdentityHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);
        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"),
            CancellationToken.None);

        return caseId;
    }

    [Fact]
    public async Task DeclareAddress_WithoutIdentity_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var declareHandler = scope.ServiceProvider.GetRequiredService<DeclareAddressHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        var act = () => declareHandler.Handle(
            caseId,
            new DeclareAddressRequest(
                "Chaussée de Louvain",
                "10",
                null,
                "1030",
                "Schaerbeek"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public async Task DeclareAddress_ValidAddress_MarksAddressDeclared()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var declareHandler = scope.ServiceProvider.GetRequiredService<DeclareAddressHandler>();

        var response = await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest(
                "Chaussée de Louvain",
                "10",
                null,
                "1030",
                "Schaerbeek"),
            CancellationToken.None);

        response.AddressDeclared.Should().BeTrue();

        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.Checklist.AddressDeclared.Should().BeTrue();
        registrationCase.DeclaredAddress!.Municipality.Should().Be("Schaerbeek");
    }

    [Fact]
    public async Task DeclareAddress_CorrectAddress_PersistsUpdate()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var declareHandler = scope.ServiceProvider.GetRequiredService<DeclareAddressHandler>();

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Rue Josaphat", "5", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Avenue Rogier", "100", "2", "1030", "Schaerbeek"),
            CancellationToken.None);

        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.DeclaredAddress!.Street.Should().Be("Avenue Rogier");
        registrationCase.DeclaredAddress.Box.Should().Be("2");
    }

    [Fact]
    public async Task RecordHousingSituation_AfterAddress_Persists()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var declareHandler = scope.ServiceProvider.GetRequiredService<DeclareAddressHandler>();
        var housingHandler = scope.ServiceProvider.GetRequiredService<RecordHousingSituationHandler>();

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        await housingHandler.Handle(
            caseId,
            new RecordHousingSituationRequest(HousingSituation.Tenant),
            CancellationToken.None);

        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.HousingSituation.Should().Be(HousingSituation.Tenant);
    }

    [Fact]
    public async Task RecordHouseholdComposition_WithSpouseAndChild_Persists()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var declareHandler = scope.ServiceProvider.GetRequiredService<DeclareAddressHandler>();
        var householdHandler = scope.ServiceProvider.GetRequiredService<RecordHouseholdCompositionHandler>();

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        await householdHandler.Handle(
            caseId,
            new RecordHouseholdCompositionRequest(
            [
                new HouseholdMemberRequest("Marc", "Lambert", new DateOnly(1985, 3, 1), HouseholdMemberRole.Spouse),
                new HouseholdMemberRequest("Léa", "Lambert", new DateOnly(2015, 9, 20), HouseholdMemberRole.Child),
            ]),
            CancellationToken.None);

        var householdRepo = scope.ServiceProvider.GetRequiredService<IHouseholdRepository>();
        var household = await householdRepo.GetByCaseIdAsync(caseId, CancellationToken.None);
        household!.Members.Should().HaveCount(2);
        household.Members.Should().Contain(m => m.Role == HouseholdMemberRole.Spouse);
        household.Members.Should().Contain(m => m.Role == HouseholdMemberRole.Child);
    }

    [Fact]
    public async Task RecordCivilStatus_Married_PersistsMarriageDetails()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var caseId = await OpenAndRecordIdentityAsync(scope.ServiceProvider);
        var civilStatusHandler = scope.ServiceProvider.GetRequiredService<RecordCivilStatusHandler>();

        await civilStatusHandler.Handle(
            caseId,
            new RecordCivilStatusRequest(
                CivilStatus.Married,
                "Marc",
                "Lambert",
                new DateOnly(2010, 8, 14),
                "Schaerbeek"),
            CancellationToken.None);

        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var personRepo = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        var person = await personRepo.GetByIdAsync(registrationCase!.PersonId!.Value, CancellationToken.None);

        person!.CivilStatus.Should().NotBeNull();
        person.CivilStatus!.Status.Should().Be(CivilStatus.Married);
        person.CivilStatus.SpouseFamilyName.Should().Be("Lambert");
    }
}
