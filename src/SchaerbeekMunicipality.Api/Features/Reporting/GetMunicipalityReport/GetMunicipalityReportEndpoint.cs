using SchaerbeekMunicipality.Application.Features.Reporting.GetMunicipalityReport;

namespace SchaerbeekMunicipality.Api.Features.Reporting.GetMunicipalityReport;

public static class GetMunicipalityReportEndpoint
{
    public static async Task<IResult> Handle(
        int? months,
        GetMunicipalityReportHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(months ?? 12, cancellationToken);
        return Results.Ok(result);
    }
}