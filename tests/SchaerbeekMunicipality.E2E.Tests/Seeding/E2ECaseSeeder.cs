using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Integration.Tests;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.E2E.Tests.Seeding;

internal static class E2ECaseSeeder
{
    internal static async Task<Guid> SeedRegistrationCaseApprovedForConfirmationAsync(MunicipalAppFactory apiFactory)
    {
        await using var scope = apiFactory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await CreateCaseReadyForDecisionAsync(services);

        await services.GetRequiredService<ApproveCaseHandler>().Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            CancellationToken.None);

        return caseId.Value;
    }

    internal static async Task EnsureJeanDupontRegisteredAsync(MunicipalAppFactory apiFactory)
    {
        await using var scope = apiFactory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var jean = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None)
                   ?? throw new InvalidOperationException("Seed NR person not found.");

        var personRepo = services.GetRequiredService<IPersonRepository>();
        var existing = await personRepo.GetByRegisterRecordIdAsync(jean.Id, CancellationToken.None);
        if (existing is not null) return;

        var person = Person.CreateFromRegisterRecord(jean);
        if (person.NationalRegisterNumber is null && jean.NationalRegisterNumber is { } nr)
            person.AssignNationalRegisterNumber(nr);

        person.UpdateDomicile(BelgianAddress.Create("Rue de la Paix", "1", null, "1030", "Schaerbeek"));
        await personRepo.AddAsync(person, CancellationToken.None);
        await services.GetRequiredService<MunicipalDbContext>().SaveChangesAsync(CancellationToken.None);
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