using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;
using SchaerbeekMunicipality.Web.Api.PersonFile;
using SchaerbeekMunicipality.Web.Api.Registration;

namespace SchaerbeekMunicipality.Web.Municipal.Components;

public sealed class NationalRegisterSearchQueries(
    IRegistrationApi registrationApi,
    IPersonFileApi personFileApi) : INationalRegisterSearchQueries
{
    public Task<NationalRegisterSearchResult> SearchLivingResidentsAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        return SearchViaRegistrationAsync(criteria, NationalRegisterSearchEligibility.LivingOnly, cancellationToken);
    }

    public Task<NationalRegisterSearchResult> SearchRegisteredResidentsAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        return SearchViaRegistrationAsync(
            criteria,
            NationalRegisterSearchEligibility.RegisteredResidentsOnly,
            cancellationToken);
    }

    public Task<NationalRegisterSearchResult> SearchAllAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        return SearchViaRegistrationAsync(criteria, NationalRegisterSearchEligibility.All, cancellationToken);
    }

    public Task<NationalRegisterSearchResult> SearchRegisteredResidentsForPersonFileAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        return SearchViaPersonFileAsync(
            criteria,
            NationalRegisterSearchEligibility.RegisteredResidentsOnly,
            cancellationToken);
    }

    public Task<NationalRegisterSearchResult> SearchAllForPersonFileAsync(
        NrSearchFormCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        return SearchViaPersonFileAsync(criteria, NationalRegisterSearchEligibility.All, cancellationToken);
    }

    private async Task<NationalRegisterSearchResult> SearchViaRegistrationAsync(
        NrSearchFormCriteria criteria,
        NationalRegisterSearchEligibility eligibility,
        CancellationToken cancellationToken)
    {
        var response = await registrationApi.SearchNationalRegisterAsync(
            NationalRegisterSearchMapper.ToRequest(criteria, eligibility),
            cancellationToken);

        return NationalRegisterSearchMapper.ToResult(response);
    }

    private async Task<NationalRegisterSearchResult> SearchViaPersonFileAsync(
        NrSearchFormCriteria criteria,
        NationalRegisterSearchEligibility eligibility,
        CancellationToken cancellationToken)
    {
        var response = await personFileApi.SearchPersonFileAsync(
            NationalRegisterSearchMapper.ToRequest(criteria, eligibility),
            cancellationToken);

        return NationalRegisterSearchMapper.ToResult(response);
    }
}
