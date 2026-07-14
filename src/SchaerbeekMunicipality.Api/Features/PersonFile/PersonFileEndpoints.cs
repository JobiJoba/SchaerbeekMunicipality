using SchaerbeekMunicipality.Api.Features.PersonFile.GetPersonFile;
using SchaerbeekMunicipality.Api.Features.PersonFile.ListPersonCases;
using SchaerbeekMunicipality.Api.Features.PersonFile.SearchPersonFile;
using SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;
using SchaerbeekMunicipality.Application.Features.PersonFile.ListPersonCases;
using SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;

namespace SchaerbeekMunicipality.Api.Features.PersonFile;

public static class PersonFileEndpoints
{
    public static IEndpointRouteBuilder MapPersonFileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/persons")
            .WithTags("PersonFile");

        group.MapGet("/search", SearchPersonFileEndpoint.Handle)
            .WithName("SearchPersonFile")
            .Produces<SearchPersonFileResponse>();

        group.MapGet("/by-nr/{nrNumber}", GetPersonFileEndpoint.HandleByNationalRegisterNumber)
            .WithName("GetPersonFileByNationalRegisterNumber")
            .Produces<GetPersonFileResponse>();

        group.MapGet("/{personId:guid}/cases", ListPersonCasesEndpoint.Handle)
            .WithName("ListPersonCases")
            .Produces<ListPersonCasesResponse>();

        group.MapGet("/{personId:guid}", GetPersonFileEndpoint.HandleById)
            .WithName("GetPersonFile")
            .Produces<GetPersonFileResponse>();

        return app;
    }
}