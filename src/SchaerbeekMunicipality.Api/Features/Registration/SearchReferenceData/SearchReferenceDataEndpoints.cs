using SchaerbeekMunicipality.Application.Features.Registration.SearchReferenceData;

namespace SchaerbeekMunicipality.Api.Features.Registration.SearchReferenceData;

public static class SearchReferenceDataEndpoints
{
    public static async Task<IResult> ListMunicipalities(
        string? search,
        SearchMunicipalitiesHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(search, cancellationToken);
        return Results.Ok(result);
    }

    public static async Task<IResult> SearchStreets(
        string postalCode,
        string? search,
        SearchStreetsHandler handler,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(postalCode)) return Results.BadRequest("Postal code is required.");

        var result = await handler.Handle(postalCode, search, cancellationToken);
        return Results.Ok(result);
    }
}