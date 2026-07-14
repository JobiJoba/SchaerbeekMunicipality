using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;
using SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Application.Features.Registration.ConfirmRegistration;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.PersonFile;

public sealed class PersonFileTests
{
    [Fact]
    public async Task RegisteredPerson_ReturnsIdentityAndRegistrationCaseLink()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var caseId = await CreateCaseReadyForDecisionAsync(scope.ServiceProvider);

        var approveHandler = scope.ServiceProvider.GetRequiredService<ApproveCaseHandler>();
        var confirmHandler = scope.ServiceProvider.GetRequiredService<ConfirmRegistrationHandler>();
        var caseRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();
        var getPersonFileHandler = scope.ServiceProvider.GetRequiredService<GetPersonFileHandler>();

        await approveHandler.Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            CancellationToken.None);

        await confirmHandler.Handle(caseId, CancellationToken.None);

        var registrationCase = await caseRepo.GetByIdAsync(caseId, CancellationToken.None);
        registrationCase!.PersonId.Should().NotBeNull();

        var personFile = await getPersonFileHandler.Handle(registrationCase.PersonId!.Value, CancellationToken.None);

        personFile.Header.GivenName.Should().Be("Sophie");
        personFile.Header.FamilyName.Should().Be("Lambert");
        personFile.Identity.Nationality.Should().Be("Belgian");
        personFile.Cases.Should().ContainSingle(c =>
            c.CaseId == caseId.Value &&
            c.Workflow == "Registration" &&
            c.Status == nameof(RegistrationCaseStatus.Registered));
    }

    [Fact]
    public async Task UnknownNationalRegisterNumber_ThrowsKeyNotFound()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var handler = scope.ServiceProvider.GetRequiredService<GetPersonFileHandler>();
        var act = () => handler.HandleByNationalRegisterNumber("85061200123", CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task PersonWithNoCases_ReturnsHeaderOnly()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var dbContext = scope.ServiceProvider.GetRequiredService<MunicipalDbContext>();
        await PopulationRegisterSeeder.SeedAsync(dbContext, CancellationToken.None);

        var personRepo = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
        var registerRepo = scope.ServiceProvider.GetRequiredService<INationalRegisterRepository>();
        var jean = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None);
        jean.Should().NotBeNull();

        jean.NationalRegisterNumber.Should().NotBeNull();
        var jeanNr = jean.NationalRegisterNumber!.Value;
        var person = await personRepo.GetByNationalRegisterNumberAsync(jeanNr, CancellationToken.None);
        person.Should().NotBeNull();

        var handler = scope.ServiceProvider.GetRequiredService<GetPersonFileHandler>();
        var personFile = await handler.Handle(person.Id, CancellationToken.None);

        personFile.Header.GivenName.Should().Be("Jean");
        personFile.Header.FamilyName.Should().Be("Dupont");
        personFile.Cases.Should().BeEmpty();
        personFile.Certificates.Should().BeEmpty();
    }

    [Theory]
    [InlineData(OfficerRole.ReceptionOfficer)]
    [InlineData(OfficerRole.BackOfficeOfficer)]
    [InlineData(OfficerRole.PoliceClerk)]
    public async Task NonPopulationOfficer_CannotViewPersonFile(OfficerRole role)
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, role);

        var handler = scope.ServiceProvider.GetRequiredService<GetPersonFileHandler>();
        var act = () => handler.Handle(PersonId.New(), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task SearchPersonFile_EnrichesPersonIdForSeededPerson()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var dbContext = scope.ServiceProvider.GetRequiredService<MunicipalDbContext>();
        await PopulationRegisterSeeder.SeedAsync(dbContext, CancellationToken.None);

        var handler = scope.ServiceProvider.GetRequiredService<SearchPersonFileHandler>();
        var response = await handler.Handle(
            new SearchNationalRegisterRequest("Sofia", "Nguyen", null),
            CancellationToken.None);

        response.Matches.Should().NotBeEmpty();
        response.Matches[0].PersonId.Should().NotBeNull();
        response.Matches[0].CanOpenPersonFile.Should().BeTrue();
    }

    [Fact]
    public async Task SearchPersonFile_NoCriteria_ReturnsAllRecordsWithPagination()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var handler = scope.ServiceProvider.GetRequiredService<SearchPersonFileHandler>();
        var response = await handler.Handle(
            new SearchNationalRegisterRequest(null, null, null),
            CancellationToken.None);

        response.TotalCount.Should().Be(5);
        response.Matches.Should().HaveCount(5);
    }

    [Fact]
    public async Task PopulationOfficer_CanLoadPersonFileViaHttp()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var dbContext = scope.ServiceProvider.GetRequiredService<MunicipalDbContext>();
        await PopulationRegisterSeeder.SeedAsync(dbContext, CancellationToken.None);

        var personRepo = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
        var registerRepo = scope.ServiceProvider.GetRequiredService<INationalRegisterRepository>();
        var sofia = await registerRepo.GetByIdAsync(NationalRegisterSeeder.SofiaNguyenId, CancellationToken.None);
        sofia!.NationalRegisterNumber.Should().NotBeNull();
        var sofiaNr = sofia.NationalRegisterNumber!.Value;
        var person = await personRepo.GetByNationalRegisterNumberAsync(sofiaNr, CancellationToken.None);

        using var client = factory.CreateApiClient();
        var response = await client.GetAsync($"/api/persons/{person!.Id.Value}");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    private static async Task<RegistrationCaseId> CreateCaseReadyForDecisionAsync(IServiceProvider services)
    {
        var recordHandler = services.GetRequiredService<RecordIdentityHandler>();
        var categoryHandler = services.GetRequiredService<SetResidenceCategoryHandler>();
        var declareHandler = services.GetRequiredService<DeclareAddressHandler>();
        var requestPoliceHandler = services.GetRequiredService<RequestPoliceVerificationHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);

        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"),
            CancellationToken.None);

        await categoryHandler.Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            CancellationToken.None);

        await RegistrationTestHelpers.SatisfyPhase9RequirementsAsync(services, caseId);

        await declareHandler.Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        var policeRequest = await requestPoliceHandler.Handle(caseId, CancellationToken.None);

        await RegistrationTestHelpers.RecordPoliceResultAsync(
            services,
            policeRequest.RequestId,
            PoliceVerificationResult.Confirmed,
            "Present");

        return caseId;
    }
}