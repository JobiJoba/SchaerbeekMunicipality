using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.AttachDocument;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ClaimDeathDeclarationCase;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.OpenDeathDeclarationCase;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.RecordDeathFacts;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ReviewHousehold;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.DeathDeclaration;

internal static class DeathDeclarationTestHelpers
{
    internal static async Task<Guid> CreateRegisteredPersonAsync(IServiceProvider services)
    {
        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var jean = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None)
                   ?? throw new InvalidOperationException("Seed person not found.");

        var personRepo = services.GetRequiredService<IPersonRepository>();
        var existing = await personRepo.GetByRegisterRecordIdAsync(jean.Id, CancellationToken.None);
        if (existing is not null) return existing.Id.Value;

        var person = Person.CreateFromRegisterRecord(jean);
        if (person.NationalRegisterNumber is null && jean.NationalRegisterNumber is { } nr)
            person.AssignNationalRegisterNumber(nr);

        person.UpdateDomicile(BelgianAddress.Create("Rue de la Paix", "1", null, "1030", "Schaerbeek"));
        await personRepo.AddAsync(person, CancellationToken.None);
        await services.GetRequiredService<MunicipalDbContext>().SaveChangesAsync();
        return person.Id.Value;
    }

    internal static async Task<DeathDeclarationCaseId> OpenAndClaimCaseAsync(IServiceProvider services, Guid personId)
    {
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var opened = await services.GetRequiredService<OpenDeathDeclarationCaseHandler>()
            .Handle(new OpenDeathDeclarationCaseRequest(personId), CancellationToken.None);
        var caseId = new DeathDeclarationCaseId(opened.CaseId);

        await services.GetRequiredService<ClaimDeathDeclarationCaseHandler>().Handle(caseId, CancellationToken.None);

        return caseId;
    }

    internal static async Task<DeathDeclarationCaseId> PrepareCaseReadyForConfirmationAsync(
        IServiceProvider services,
        Guid personId)
    {
        var caseId = await OpenAndClaimCaseAsync(services, personId);
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        await services.GetRequiredService<RecordDeathFactsHandler>().Handle(
            caseId,
            new RecordDeathFactsRequest(yesterday, "CHU Saint-Pierre", false, InformantRelationship.Spouse),
            CancellationToken.None);

        await using var pdf = new MemoryStream("%PDF-1.4 death"u8.ToArray());
        await services.GetRequiredService<AttachDocumentHandler>().Handle(
            caseId,
            "death-act.pdf",
            pdf,
            CancellationToken.None);

        await services.GetRequiredService<ReviewHouseholdHandler>().Handle(caseId, CancellationToken.None);

        return caseId;
    }
}
