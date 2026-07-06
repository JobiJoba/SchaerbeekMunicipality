using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.ClaimRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordBirthInformation;
using SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

internal static class RegistrationTestHelpers
{
    internal static async Task<RegistrationCaseId> OpenAndClaimCaseAsync(
        IServiceProvider services,
        VisitReason visitReason = VisitReason.FirstRegistration,
        CancellationToken cancellationToken = default)
    {
        var openHandler = services.GetRequiredService<OpenRegistrationCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimRegistrationCaseHandler>();

        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(visitReason, null),
            cancellationToken);

        var caseId = new RegistrationCaseId(opened.CaseId);
        await claimHandler.Handle(caseId, cancellationToken);
        return caseId;
    }

    internal static async Task AttachPassportViaApiAsync(HttpClient client, Guid caseId)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([0x25, 0x50, 0x44, 0x46]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        content.Add(fileContent, "file", "passport.pdf");

        var response = await client.PostAsync(
            $"/api/registration/cases/{caseId}/documents?documentType={DocumentType.Passport}",
            content);

        response.EnsureSuccessStatusCode();
    }

    internal static async Task AttachIdentityDocumentAsync(
        IServiceProvider services,
        RegistrationCaseId caseId,
        CancellationToken cancellationToken = default)
    {
        var attachHandler = services.GetRequiredService<AttachDocumentHandler>();
        await using var stream = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        await attachHandler.Handle(
            caseId,
            DocumentType.Passport,
            "passport.pdf",
            stream,
            cancellationToken);
    }

    internal static async Task SatisfyPhase9RequirementsAsync(
        IServiceProvider services,
        RegistrationCaseId caseId,
        CancellationToken cancellationToken = default)
    {
        var birthHandler = services.GetRequiredService<RecordBirthInformationHandler>();
        var attachHandler = services.GetRequiredService<AttachDocumentHandler>();

        await birthHandler.Handle(
            caseId,
            new RecordBirthInformationRequest("Brussels", "Belgium"),
            cancellationToken);

        await AttachDocumentAsync(
            attachHandler,
            caseId,
            DocumentType.Passport,
            "passport.pdf",
            cancellationToken);

        await AttachDocumentAsync(
            attachHandler,
            caseId,
            DocumentType.BirthCertificate,
            "birth-certificate.pdf",
            cancellationToken);
    }

    internal static async Task RecordPoliceResultAsync(
        IServiceProvider services,
        Guid requestId,
        PoliceVerificationResult result,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        SetRole(services, OfficerRole.PoliceClerk);

        await services.GetRequiredService<RecordPoliceResultHandler>().Handle(
            PoliceVerificationRequestId.From(requestId),
            new RecordPoliceResultRequest(result, notes),
            cancellationToken);

        SetRole(services, OfficerRole.PopulationOfficer);
    }

    internal static void SetRole(IServiceProvider services, OfficerRole role)
    {
        var officer = services.GetRequiredService<ICurrentOfficer>();
        officer.SetRole(role);
    }

    internal static void SelectPopulationOfficer(IServiceProvider services, bool secondary = false)
    {
        var officer = services.GetRequiredService<ICurrentOfficer>();
        officer.SelectOfficer(
            secondary
                ? CurrentOfficer.SecondaryPopulationOfficerId
                : CurrentOfficer.PopulationOfficerId);
    }

    internal static void ImpersonatePopulationOfficer(
        IServiceProvider services,
        Guid officerId,
        string displayName)
    {
        var officer = services.GetRequiredService<ICurrentOfficer>();
        officer.SelectOfficer(officerId);
    }

    private static async Task AttachDocumentAsync(
        AttachDocumentHandler attachHandler,
        RegistrationCaseId caseId,
        DocumentType documentType,
        string fileName,
        CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        await attachHandler.Handle(
            caseId,
            documentType,
            fileName,
            stream,
            cancellationToken);
    }
}
