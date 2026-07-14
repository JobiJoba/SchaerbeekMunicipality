using SchaerbeekMunicipality.Api.Features.RegisterAmendment.ApplyRegisterAmendment;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.ApproveRegisterAmendment;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.AttachDocument;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.ClaimRegisterAmendmentCase;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.DownloadDocument;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.GetRegisterAmendmentCase;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.ListRegisterAmendmentCases;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.OpenRegisterAmendmentCase;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.RecordProposedAmendment;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.RejectRegisterAmendment;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.ReleaseCaseLock;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.RemoveDocument;
using SchaerbeekMunicipality.Api.Features.RegisterAmendment.SubmitRegisterAmendmentForReview;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment;

public static class RegisterAmendmentEndpoints
{
    public static IEndpointRouteBuilder MapRegisterAmendmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/register-amendments")
            .WithTags("RegisterAmendment");

        group.MapGet("/cases", ListRegisterAmendmentCasesEndpoint.Handle)
            .WithName("ListRegisterAmendmentCases");

        group.MapPost("/cases", OpenRegisterAmendmentCaseEndpoint.Handle)
            .WithName("OpenRegisterAmendmentCase");

        group.MapGet("/cases/{id:guid}", GetRegisterAmendmentCaseEndpoint.Handle)
            .WithName("GetRegisterAmendmentCase");

        group.MapPost("/cases/{id:guid}/claim", ClaimRegisterAmendmentCaseEndpoint.Handle)
            .WithName("ClaimRegisterAmendmentCase");

        group.MapPost("/cases/{id:guid}/auto-claim", AutoClaimRegisterAmendmentCaseEndpoint.Handle)
            .WithName("AutoClaimRegisterAmendmentCase");

        group.MapPost("/cases/{id:guid}/release-lock", ReleaseCaseLockEndpoint.Handle)
            .WithName("ReleaseRegisterAmendmentCaseLock");

        group.MapPut("/cases/{id:guid}/proposed-changes", RecordProposedAmendmentEndpoint.Handle)
            .WithName("RecordProposedAmendment");

        group.MapPost("/cases/{id:guid}/documents", AttachDocumentEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachRegisterAmendmentDocument");

        group.MapGet("/cases/{id:guid}/documents/{documentId:guid}", DownloadDocumentEndpoint.Handle)
            .WithName("DownloadRegisterAmendmentDocument");

        group.MapDelete("/cases/{id:guid}/documents/{documentId:guid}", RemoveDocumentEndpoint.Handle)
            .WithName("RemoveRegisterAmendmentDocument");

        group.MapPost("/cases/{id:guid}/submit", SubmitRegisterAmendmentForReviewEndpoint.Handle)
            .WithName("SubmitRegisterAmendmentForReview");

        group.MapPost("/cases/{id:guid}/approve", ApproveRegisterAmendmentEndpoint.Handle)
            .WithName("ApproveRegisterAmendment");

        group.MapPost("/cases/{id:guid}/reject", RejectRegisterAmendmentEndpoint.Handle)
            .WithName("RejectRegisterAmendment");

        group.MapPost("/cases/{id:guid}/apply", ApplyRegisterAmendmentEndpoint.Handle)
            .WithName("ApplyRegisterAmendment");

        return app;
    }
}
