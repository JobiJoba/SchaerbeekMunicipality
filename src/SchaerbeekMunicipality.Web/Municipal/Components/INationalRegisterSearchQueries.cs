namespace SchaerbeekMunicipality.Web.Municipal.Components;

public interface INationalRegisterSearchQueries
{
    Task<NationalRegisterSearchResult> SearchLivingResidentsAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<NationalRegisterSearchResult> SearchRegisteredResidentsAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<NationalRegisterSearchResult> SearchAllAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<NationalRegisterSearchResult> SearchRegisteredResidentsForPersonFileAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<NationalRegisterSearchResult> SearchAllForPersonFileAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default);
}
