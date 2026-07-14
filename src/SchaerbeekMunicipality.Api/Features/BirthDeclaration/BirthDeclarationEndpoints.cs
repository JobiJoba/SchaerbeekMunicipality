using SchaerbeekMunicipality.Api.Features.BirthDeclaration.AttachDocument;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.ClaimBirthDeclarationCase;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.ConfirmBirthDeclaration;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.DownloadDocument;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.GetBirthDeclarationCase;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.GetBirthDeclarationChecklist;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.LinkParent;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.ListBirthDeclarationCases;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.OpenBirthDeclarationCase;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.RecordChildDetails;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.RejectBirthDeclaration;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.ReleaseCaseLock;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.RemoveDocument;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.ResumeBirthDeclaration;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.SetDeclarationHousehold;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.SuspendBirthDeclaration;
using SchaerbeekMunicipality.Api.Features.BirthDeclaration.UnlinkParent;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration;

public static class BirthDeclarationEndpoints
{
    public static IEndpointRouteBuilder MapBirthDeclarationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/birth-declarations");

        group.MapGet("/cases", ListBirthDeclarationCasesEndpoint.Handle)
            .WithName("ListBirthDeclarationCases")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases", OpenBirthDeclarationCaseEndpoint.Handle)
            .WithName("OpenBirthDeclarationCase")
            .WithTags("BirthDeclaration");

        group.MapGet("/cases/{id:guid}", GetBirthDeclarationCaseEndpoint.Handle)
            .WithName("GetBirthDeclarationCase")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/claim", ClaimBirthDeclarationCaseEndpoint.Handle)
            .WithName("ClaimBirthDeclarationCase")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/auto-claim", AutoClaimBirthDeclarationCaseEndpoint.Handle)
            .WithName("AutoClaimBirthDeclarationCase")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/release-lock", ReleaseCaseLockEndpoint.Handle)
            .WithName("ReleaseBirthDeclarationCaseLock")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/child-details", RecordChildDetailsEndpoint.Handle)
            .WithName("RecordChildDetails")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/parents/link", LinkParentEndpoint.Handle)
            .WithName("LinkParent")
            .WithTags("BirthDeclaration");

        group.MapDelete("/cases/{id:guid}/parents/{personId:guid}", UnlinkParentEndpoint.Handle)
            .WithName("UnlinkParent")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/household", SetDeclarationHouseholdEndpoint.Handle)
            .WithName("SetDeclarationHousehold")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/documents", AttachDocumentEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachBirthDeclarationDocument")
            .WithTags("BirthDeclaration");

        group.MapGet("/cases/{id:guid}/documents/{documentId:guid}", DownloadDocumentEndpoint.Handle)
            .WithName("DownloadBirthDeclarationDocument")
            .WithTags("BirthDeclaration");

        group.MapDelete("/cases/{id:guid}/documents/{documentId:guid}", RemoveDocumentEndpoint.Handle)
            .WithName("RemoveBirthDeclarationDocument")
            .WithTags("BirthDeclaration");

        group.MapGet("/cases/{id:guid}/checklist", GetBirthDeclarationChecklistEndpoint.Handle)
            .WithName("GetBirthDeclarationChecklist")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/confirm", ConfirmBirthDeclarationEndpoint.Handle)
            .WithName("ConfirmBirthDeclaration")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/reject", RejectBirthDeclarationEndpoint.Handle)
            .WithName("RejectBirthDeclaration")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/suspend", SuspendBirthDeclarationEndpoint.Handle)
            .WithName("SuspendBirthDeclaration")
            .WithTags("BirthDeclaration");

        group.MapPost("/cases/{id:guid}/resume", ResumeBirthDeclarationEndpoint.Handle)
            .WithName("ResumeBirthDeclaration")
            .WithTags("BirthDeclaration");

        return app;
    }
}