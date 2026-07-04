using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.CorrectIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordCivilStatus;
using SchaerbeekMunicipality.Web.Features.Registration.RecordHouseholdComposition;
using SchaerbeekMunicipality.Web.Features.Registration.RecordHousingSituation;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.RecordImmigrationDecision;
using SchaerbeekMunicipality.Web.Features.Registration.RecordResidencePermit;
using SchaerbeekMunicipality.Web.Features.Registration.RemoveDocument;
using SchaerbeekMunicipality.Web.Features.Registration.SearchReferenceData;
using SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

namespace SchaerbeekMunicipality.Web.Features.Registration;

public static class RegistrationEndpoints
{
    public static IEndpointRouteBuilder MapRegistrationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/registration");

        group.MapGet("/cases", ListRegistrationCasesEndpoint.Handle)
            .WithName("ListRegistrationCases")
            .WithTags("Registration");

        group.MapPost("/cases", OpenRegistrationCaseEndpoint.Handle)
            .WithName("OpenRegistrationCase")
            .WithTags("Registration");

        group.MapGet("/cases/{id:guid}", GetRegistrationCaseEndpoint.Handle)
            .WithName("GetRegistrationCase")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/identity", RecordIdentityEndpoint.Handle)
            .WithName("RecordIdentity")
            .WithTags("Registration");

        group.MapPut("/cases/{id:guid}/identity", CorrectIdentityEndpoint.Handle)
            .WithName("CorrectIdentity")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/residence-category", SetResidenceCategoryEndpoint.Handle)
            .WithName("SetResidenceCategory")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/residence-permit", RecordResidencePermitEndpoint.Handle)
            .WithName("RecordResidencePermit")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/immigration-decision", RecordImmigrationDecisionEndpoint.Handle)
            .WithName("RecordImmigrationDecision")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/address", DeclareAddressEndpoint.Handle)
            .WithName("DeclareAddress")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/housing-situation", RecordHousingSituationEndpoint.Handle)
            .WithName("RecordHousingSituation")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/household", RecordHouseholdCompositionEndpoint.Handle)
            .WithName("RecordHouseholdComposition")
            .WithTags("Registration");

        group.MapPost("/cases/{id:guid}/civil-status", RecordCivilStatusEndpoint.Handle)
            .WithName("RecordCivilStatus")
            .WithTags("Registration");

        group.MapGet("/municipalities", SearchReferenceDataEndpoints.ListMunicipalities)
            .WithName("ListMunicipalities")
            .WithTags("ReferenceData");

        group.MapGet("/streets", SearchReferenceDataEndpoints.SearchStreets)
            .WithName("SearchStreets")
            .WithTags("ReferenceData");

        group.MapPost("/cases/{id:guid}/documents", AttachDocumentEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachDocument")
            .WithTags("Registration");

        group.MapDelete("/cases/{id:guid}/documents/{documentId:guid}", RemoveDocumentEndpoint.Handle)
            .WithName("RemoveDocument")
            .WithTags("Registration");

        return app;
    }
}
