using SchaerbeekMunicipality.Api.Features.IdentityDocuments.AdvanceDocumentRequestStatus;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.AttachApplicantPhoto;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.CancelDocumentRequest;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.ClaimDocumentRequestCase;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.DownloadDocument;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.GetDocumentRequestCase;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.IssueDocument;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.ListDocumentRequestCases;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.OpenDocumentRequestCase;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.RecordFeePayment;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.ReleaseCaseLock;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.RemoveDocument;
using SchaerbeekMunicipality.Api.Features.IdentityDocuments.ResolveRegisteredPerson;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments;

public static class IdentityDocumentsEndpoints
{
    public static IEndpointRouteBuilder MapIdentityDocumentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity-documents");

        group.MapGet("/requests", ListDocumentRequestCasesEndpoint.Handle)
            .WithName("ListDocumentRequestCases")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests", OpenDocumentRequestCaseEndpoint.Handle)
            .WithName("OpenDocumentRequestCase")
            .WithTags("IdentityDocuments");

        group.MapPost("/resolve-person", ResolveRegisteredPersonEndpoint.Handle)
            .WithName("ResolveDocumentRequestPerson")
            .WithTags("IdentityDocuments");

        group.MapGet("/requests/{id:guid}", GetDocumentRequestCaseEndpoint.Handle)
            .WithName("GetDocumentRequestCase")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/claim", ClaimDocumentRequestCaseEndpoint.Handle)
            .WithName("ClaimDocumentRequestCase")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/auto-claim", AutoClaimDocumentRequestCaseEndpoint.Handle)
            .WithName("AutoClaimDocumentRequestCase")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/release-lock", ReleaseCaseLockEndpoint.Handle)
            .WithName("ReleaseDocumentRequestCaseLock")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/photo", AttachApplicantPhotoEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachApplicantPhoto")
            .WithTags("IdentityDocuments");

        group.MapGet("/requests/{id:guid}/documents/{documentId:guid}", DownloadDocumentEndpoint.Handle)
            .WithName("DownloadDocumentRequestDocument")
            .WithTags("IdentityDocuments");

        group.MapDelete("/requests/{id:guid}/documents/{documentId:guid}", RemoveDocumentEndpoint.Handle)
            .WithName("RemoveDocumentRequestDocument")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/fee-payment", RecordFeePaymentEndpoint.Handle)
            .WithName("RecordDocumentRequestFeePayment")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/advance", AdvanceDocumentRequestStatusEndpoint.Handle)
            .WithName("AdvanceDocumentRequestStatus")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/issue", IssueDocumentEndpoint.Handle)
            .WithName("IssueDocument")
            .WithTags("IdentityDocuments");

        group.MapPost("/requests/{id:guid}/cancel", CancelDocumentRequestEndpoint.Handle)
            .WithName("CancelDocumentRequest")
            .WithTags("IdentityDocuments");

        return app;
    }
}
