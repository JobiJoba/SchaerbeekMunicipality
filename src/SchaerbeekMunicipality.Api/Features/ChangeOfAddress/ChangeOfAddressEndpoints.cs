using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.AttachDocument;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ClaimChangeOfAddressCase;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ConfirmAddressChange;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.DeclareNewAddress;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.DownloadDocument;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.GetChangeOfAddressCase;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.GetChangeOfAddressChecklist;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ListChangeOfAddressCases;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.OpenChangeOfAddressCase;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.RejectChangeOfAddress;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ReleaseCaseLock;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.RemoveDocument;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.RequestPoliceVerification;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ResolveRegisteredPerson;
using SchaerbeekMunicipality.Api.Features.ChangeOfAddress.UpdateHouseholdForMove;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress;

public static class ChangeOfAddressEndpoints
{
    public static IEndpointRouteBuilder MapChangeOfAddressEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/change-of-address");

        group.MapGet("/cases", ListChangeOfAddressCasesEndpoint.Handle)
            .WithName("ListChangeOfAddressCases")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases", OpenChangeOfAddressCaseEndpoint.Handle)
            .WithName("OpenChangeOfAddressCase")
            .WithTags("ChangeOfAddress");

        group.MapPost("/resolve-person", ResolveRegisteredPersonEndpoint.Handle)
            .WithName("ResolveRegisteredPerson")
            .WithTags("ChangeOfAddress");

        group.MapGet("/cases/{id:guid}", GetChangeOfAddressCaseEndpoint.Handle)
            .WithName("GetChangeOfAddressCase")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/claim", ClaimChangeOfAddressCaseEndpoint.Handle)
            .WithName("ClaimChangeOfAddressCase")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/auto-claim", AutoClaimChangeOfAddressCaseEndpoint.Handle)
            .WithName("AutoClaimChangeOfAddressCase")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/release-lock", ReleaseCaseLockEndpoint.Handle)
            .WithName("ReleaseChangeOfAddressCaseLock")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/new-address", DeclareNewAddressEndpoint.Handle)
            .WithName("DeclareNewAddress")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/household", UpdateHouseholdForMoveEndpoint.Handle)
            .WithName("UpdateHouseholdForMove")
            .WithTags("ChangeOfAddress");

        group.MapDelete("/cases/{id:guid}/household/{personId:guid}", UnlinkHouseholdMemberEndpoint.Handle)
            .WithName("UnlinkHouseholdMember")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/police-verification", RequestPoliceVerificationEndpoint.Handle)
            .WithName("RequestChangeOfAddressPoliceVerification")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/documents", AttachDocumentEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachChangeOfAddressDocument")
            .WithTags("ChangeOfAddress");

        group.MapGet("/cases/{id:guid}/documents/{documentId:guid}", DownloadDocumentEndpoint.Handle)
            .WithName("DownloadChangeOfAddressDocument")
            .WithTags("ChangeOfAddress");

        group.MapDelete("/cases/{id:guid}/documents/{documentId:guid}", RemoveDocumentEndpoint.Handle)
            .WithName("RemoveChangeOfAddressDocument")
            .WithTags("ChangeOfAddress");

        group.MapGet("/cases/{id:guid}/checklist", GetChangeOfAddressChecklistEndpoint.Handle)
            .WithName("GetChangeOfAddressChecklist")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/confirm", ConfirmAddressChangeEndpoint.Handle)
            .WithName("ConfirmAddressChange")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/reject", RejectChangeOfAddressEndpoint.Handle)
            .WithName("RejectChangeOfAddress")
            .WithTags("ChangeOfAddress");

        return app;
    }
}