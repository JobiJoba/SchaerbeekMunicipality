namespace SchaerbeekMunicipality.Domain.ReferenceData;

public interface IReferenceDataRepository
{
    Task<IReadOnlyList<MunicipalityReference>> ListMunicipalitiesAsync(
        string? search,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StreetReference>> SearchStreetsAsync(
        string postalCode,
        string? search,
        CancellationToken cancellationToken);
}