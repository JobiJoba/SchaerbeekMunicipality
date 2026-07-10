using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.AttachDocument;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.ClaimChangeOfAddressCase;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.ConfirmAddressChange;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.DeclareNewAddress;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.DownloadDocument;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.GetChangeOfAddressCase;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.GetChangeOfAddressChecklist;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.ListChangeOfAddressCases;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.OpenChangeOfAddressCase;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.RejectChangeOfAddress;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.ReleaseCaseLock;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.RemoveDocument;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.RequestPoliceVerification;
using SchaerbeekMunicipality.Web.Features.ChangeOfAddress.UpdateHouseholdForMove;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress;

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

        group.MapGet("/cases/{id:guid}", GetChangeOfAddressCaseEndpoint.Handle)
            .WithName("GetChangeOfAddressCase")
            .WithTags("ChangeOfAddress");

        group.MapPost("/cases/{id:guid}/claim", ClaimChangeOfAddressCaseEndpoint.Handle)
            .WithName("ClaimChangeOfAddressCase")
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
