using System.Net;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Web.Api;

internal sealed class MunicipalDocumentDownloadForwarder(
    HttpClient httpClient,
    ICurrentOfficer currentOfficer)
{
    public async Task<IResult> ForwardAsync(string relativeUri, CancellationToken cancellationToken)
    {
        ApplyOfficerHeaders();

        using var response = await httpClient.GetAsync(
            relativeUri,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Results.NotFound();

        await ApiException.ThrowIfErrorAsync(response, cancellationToken);

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var buffer = new MemoryStream();
        await responseStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        return Results.Stream(buffer, contentType, enableRangeProcessing: true);
    }

    private void ApplyOfficerHeaders()
    {
        httpClient.DefaultRequestHeaders.Remove(DemoOfficerHeaders.OfficerId);
        httpClient.DefaultRequestHeaders.Remove(DemoOfficerHeaders.OfficerRole);
        httpClient.DefaultRequestHeaders.Remove(DemoOfficerHeaders.OfficerName);

        httpClient.DefaultRequestHeaders.Add(DemoOfficerHeaders.OfficerId, currentOfficer.OfficerId.ToString());
        httpClient.DefaultRequestHeaders.Add(DemoOfficerHeaders.OfficerRole, currentOfficer.Role.ToString());
        httpClient.DefaultRequestHeaders.Add(DemoOfficerHeaders.OfficerName, currentOfficer.DisplayName);
    }
}
