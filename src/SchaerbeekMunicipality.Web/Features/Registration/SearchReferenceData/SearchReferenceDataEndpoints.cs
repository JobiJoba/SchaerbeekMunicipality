using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Web.Features.Registration.SearchReferenceData;

public sealed record MunicipalityDto(string PostalCode, string Name);

public sealed record StreetDto(int Id, string PostalCode, string Name);

public sealed class SearchMunicipalitiesHandler(IReferenceDataRepository referenceDataRepository)
{
    public async Task<IReadOnlyList<MunicipalityDto>> Handle(
        string? search,
        CancellationToken cancellationToken)
    {
        var municipalities = await referenceDataRepository.ListMunicipalitiesAsync(search, cancellationToken);

        return municipalities
            .Select(m => new MunicipalityDto(m.PostalCode, m.Name))
            .ToList();
    }
}

public sealed class SearchStreetsHandler(IReferenceDataRepository referenceDataRepository)
{
    public async Task<IReadOnlyList<StreetDto>> Handle(
        string postalCode,
        string? search,
        CancellationToken cancellationToken)
    {
        var streets = await referenceDataRepository.SearchStreetsAsync(postalCode, search, cancellationToken);

        return streets
            .Select(s => new StreetDto(s.Id, s.PostalCode, s.Name))
            .ToList();
    }
}

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
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return Results.BadRequest("Postal code is required.");
        }

        var result = await handler.Handle(postalCode, search, cancellationToken);
        return Results.Ok(result);
    }
}
