using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Reporting.GetMunicipalityReport;

namespace SchaerbeekMunicipality.Web.Api.Reporting;

public sealed class ReportingApiClient(HttpClient httpClient, ICurrentOfficer currentOfficer)
    : MunicipalApiClientBase(httpClient, currentOfficer), IReportingApi
{
    private const string BasePath = "/api/reports";

    public Task<GetMunicipalityReportResponse> GetMunicipalityReportAsync(
        int months = 12,
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<GetMunicipalityReportResponse>($"{BasePath}/municipality?months={months}",
            cancellationToken);
    }
}