using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.ClaimBirthDeclarationCase;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.LinkParent;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.OpenBirthDeclarationCase;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.RecordChildDetails;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.SetDeclarationHousehold;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.BirthDeclaration;

internal static class BirthDeclarationTestHelpers
{
    internal static void SetRole(IServiceProvider services, OfficerRole role)
    {
        RegistrationTestHelpers.SetRole(services, role);
    }

    internal static async Task<BirthDeclarationCaseId> OpenAndClaimCaseAsync(IServiceProvider services)
    {
        SetRole(services, OfficerRole.PopulationOfficer);

        var openHandler = services.GetRequiredService<OpenBirthDeclarationCaseHandler>();
        var opened = await openHandler.Handle(CancellationToken.None);
        var caseId = new BirthDeclarationCaseId(opened.CaseId);

        var claimHandler = services.GetRequiredService<ClaimBirthDeclarationCaseHandler>();
        await claimHandler.Handle(caseId, CancellationToken.None);

        return caseId;
    }

    internal static async Task<BirthDeclarationCaseId> PrepareCaseReadyForConfirmationAsync(IServiceProvider services)
    {
        var caseId = await OpenAndClaimCaseAsync(services);
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        await services.GetRequiredService<RecordChildDetailsHandler>().Handle(
            caseId,
            new RecordChildDetailsRequest("Amélie", "Dupont", NewbornSex.Female, yesterday, null, "CHU Saint-Pierre"),
            CancellationToken.None);

        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var jeanDupont = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None)
            ?? throw new InvalidOperationException("Seed parent not found.");

        await services.GetRequiredService<LinkParentHandler>().Handle(
            caseId,
            new LinkParentRequest(jeanDupont.Id.Value, ParentRole.Father),
            CancellationToken.None);

        await using var pdf = new MemoryStream("%PDF-1.4 birth"u8.ToArray());
        await services.GetRequiredService<AttachDocumentHandler>().Handle(
            caseId,
            "medical.pdf",
            pdf,
            CancellationToken.None);

        await services.GetRequiredService<SetDeclarationHouseholdHandler>().Handle(
            caseId,
            new SetDeclarationHouseholdRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        return caseId;
    }
}
