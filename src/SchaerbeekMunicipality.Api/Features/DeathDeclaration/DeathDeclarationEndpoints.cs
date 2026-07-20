using SchaerbeekMunicipality.Api.Features.DeathDeclaration.AttachDocument;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.ClaimDeathDeclarationCase;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.ConfirmRadiation;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.DownloadDocument;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.GetDeathDeclarationCase;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.GetDeathDeclarationChecklist;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.ListDeathDeclarationCases;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.OpenDeathDeclarationCase;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.RecordDeathFacts;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.RejectDeathDeclaration;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.ReleaseCaseLock;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.RemoveDocument;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.ResumeDeathDeclaration;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.ReviewHousehold;
using SchaerbeekMunicipality.Api.Features.DeathDeclaration.SuspendDeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration;

public static class DeathDeclarationEndpoints
{
    public static IEndpointRouteBuilder MapDeathDeclarationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/death-declarations");

        group.MapGet("/cases", ListDeathDeclarationCasesEndpoint.Handle)
            .WithName("ListDeathDeclarationCases")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases", OpenDeathDeclarationCaseEndpoint.Handle)
            .WithName("OpenDeathDeclarationCase")
            .WithTags("DeathDeclaration");

        group.MapGet("/cases/{id:guid}", GetDeathDeclarationCaseEndpoint.Handle)
            .WithName("GetDeathDeclarationCase")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/claim", ClaimDeathDeclarationCaseEndpoint.Handle)
            .WithName("ClaimDeathDeclarationCase")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/auto-claim", AutoClaimDeathDeclarationCaseEndpoint.Handle)
            .WithName("AutoClaimDeathDeclarationCase")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/release-lock", ReleaseCaseLockEndpoint.Handle)
            .WithName("ReleaseDeathDeclarationCaseLock")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/death-facts", RecordDeathFactsEndpoint.Handle)
            .WithName("RecordDeathFacts")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/review-household", ReviewHouseholdEndpoint.Handle)
            .WithName("ReviewDeathDeclarationHousehold")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/documents", AttachDocumentEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachDeathDeclarationDocument")
            .WithTags("DeathDeclaration");

        group.MapGet("/cases/{id:guid}/documents/{documentId:guid}", DownloadDocumentEndpoint.Handle)
            .WithName("DownloadDeathDeclarationDocument")
            .WithTags("DeathDeclaration");

        group.MapDelete("/cases/{id:guid}/documents/{documentId:guid}", RemoveDocumentEndpoint.Handle)
            .WithName("RemoveDeathDeclarationDocument")
            .WithTags("DeathDeclaration");

        group.MapGet("/cases/{id:guid}/checklist", GetDeathDeclarationChecklistEndpoint.Handle)
            .WithName("GetDeathDeclarationChecklist")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/confirm", ConfirmRadiationEndpoint.Handle)
            .WithName("ConfirmRadiation")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/reject", RejectDeathDeclarationEndpoint.Handle)
            .WithName("RejectDeathDeclaration")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/suspend", SuspendDeathDeclarationEndpoint.Handle)
            .WithName("SuspendDeathDeclaration")
            .WithTags("DeathDeclaration");

        group.MapPost("/cases/{id:guid}/resume", ResumeDeathDeclarationEndpoint.Handle)
            .WithName("ResumeDeathDeclaration")
            .WithTags("DeathDeclaration");

        return app;
    }
}
