using SchaerbeekMunicipality.Api.Features.Reporting.GetMunicipalityReport;
using SchaerbeekMunicipality.Application.Features.Reporting.GetMunicipalityReport;

namespace SchaerbeekMunicipality.Api.Features.Reporting;

public static class ReportingEndpoints
{
    public static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports")
            .WithTags("Reporting");

        group.MapGet("/municipality", GetMunicipalityReportEndpoint.Handle)
            .WithName("GetMunicipalityReport")
            .Produces<GetMunicipalityReportResponse>();

        return app;
    }
}
