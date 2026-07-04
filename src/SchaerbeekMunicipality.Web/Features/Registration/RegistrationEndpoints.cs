using SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

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

        group.MapPost("/cases/{id:guid}/documents", AttachDocumentEndpoint.Handle)
            .DisableAntiforgery()
            .WithName("AttachDocument")
            .WithTags("Registration");

        return app;
    }
}
