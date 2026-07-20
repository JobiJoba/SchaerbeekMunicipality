using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;
using SchaerbeekMunicipality.Application.Features.PersonFile.ListPersonCases;
using SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Web.Api.PersonFile;

public sealed class PersonFileApiClient(HttpClient httpClient, ICurrentOfficer currentOfficer)
    : MunicipalApiClientBase(httpClient, currentOfficer), IPersonFileApi
{
    private const string BasePath = "/api/persons";

    public Task<GetPersonFileResponse> GetPersonFileAsync(
        Guid personId,
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<GetPersonFileResponse>($"{BasePath}/{personId}", cancellationToken);
    }

    public Task<GetPersonFileResponse> GetPersonFileByNationalRegisterNumberAsync(
        string nationalRegisterNumber,
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<GetPersonFileResponse>(
            $"{BasePath}/by-nr/{Uri.EscapeDataString(nationalRegisterNumber)}",
            cancellationToken);
    }

    public Task<SearchPersonFileResponse> SearchPersonFileAsync(
        SearchNationalRegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            ("GivenName", request.GivenName),
            ("FamilyName", request.FamilyName),
            ("BirthDate", request.BirthDate?.ToString("O")),
            ("Page", request.Page.ToString()),
            ("PageSize", request.PageSize.ToString()),
            ("ExcludeDeceased", request.ExcludeDeceased ? "true" : null));

        return GetJsonAsync<SearchPersonFileResponse>($"{BasePath}/search{query}", cancellationToken);
    }

    public Task<ListPersonCasesResponse> ListPersonCasesAsync(
        Guid personId,
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<ListPersonCasesResponse>($"{BasePath}/{personId}/cases", cancellationToken);
    }
}