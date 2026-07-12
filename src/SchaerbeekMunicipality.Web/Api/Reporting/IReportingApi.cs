using SchaerbeekMunicipality.Application.Features.Reporting.GetMunicipalityReport;

namespace SchaerbeekMunicipality.Web.Api.Reporting;

public interface IReportingApi
{
    Task<GetMunicipalityReportResponse> GetMunicipalityReportAsync(
        int months = 12,
        CancellationToken cancellationToken = default);
}
