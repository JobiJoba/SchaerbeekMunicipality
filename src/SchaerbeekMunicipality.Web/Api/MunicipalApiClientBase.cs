using System.Net;
using System.Net.Http.Json;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Web.Api;

public abstract class MunicipalApiClientBase(HttpClient httpClient, ICurrentOfficer currentOfficer)
{
    protected Task<T> GetJsonAsync<T>(string uri, CancellationToken cancellationToken) =>
        SendJsonAsync<T>(ct => httpClient.GetAsync(uri, ct), cancellationToken);

    protected Task<string> GetStringAsync(string uri, CancellationToken cancellationToken) =>
        SendStringAsync(ct => httpClient.GetAsync(uri, ct), cancellationToken);

    protected Task<T> PostJsonAsync<T>(string uri, CancellationToken cancellationToken) =>
        SendJsonAsync<T>(ct => httpClient.PostAsync(uri, content: null, ct), cancellationToken);

    protected Task<TResponse> PostJsonAsync<TRequest, TResponse>(
        string uri,
        TRequest request,
        CancellationToken cancellationToken) =>
        SendJsonAsync<TResponse>(
            ct => httpClient.PostAsJsonAsync(uri, request, ct),
            cancellationToken);

    protected Task<TResponse> PutJsonAsync<TRequest, TResponse>(
        string uri,
        TRequest request,
        CancellationToken cancellationToken) =>
        SendJsonAsync<TResponse>(
            ct => httpClient.PutAsJsonAsync(uri, request, ct),
            cancellationToken);

    protected Task<T> DeleteJsonAsync<T>(string uri, CancellationToken cancellationToken) =>
        SendJsonAsync<T>(ct => httpClient.DeleteAsync(uri, ct), cancellationToken);

    protected async Task<Stream> DownloadStreamAsync(string uri, CancellationToken cancellationToken)
    {
        ApplyOfficerHeaders();
        var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await ApiException.ThrowIfErrorAsync(response, cancellationToken);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;
        return buffer;
    }

    protected async Task<TResponse> PostMultipartFileAsync<TResponse>(
        string uri,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        content.Add(fileContent, "file", fileName);

        ApplyOfficerHeaders();
        var response = await httpClient.PostAsync(uri, content, cancellationToken);
        await ApiException.ThrowIfErrorAsync(response, cancellationToken);

        return (await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken))!;
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

    protected Task<TResponse?> PostJsonOptionalAsync<TResponse>(
        string uri,
        CancellationToken cancellationToken) =>
        SendJsonOptionalAsync<TResponse>(ct => httpClient.PostAsync(uri, content: null, ct), cancellationToken);

    protected static string BuildQuery(params (string Key, string? Value)[] parameters)
    {
        var pairs = parameters
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter =>
                $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value!)}")
            .ToArray();

        return pairs.Length == 0 ? string.Empty : "?" + string.Join("&", pairs);
    }

    private async Task<T?> SendJsonOptionalAsync<T>(
        Func<CancellationToken, Task<HttpResponseMessage>> send,
        CancellationToken cancellationToken)
    {
        ApplyOfficerHeaders();
        var response = await send(cancellationToken);
        await ApiException.ThrowIfErrorAsync(response, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private async Task<T> SendJsonAsync<T>(
        Func<CancellationToken, Task<HttpResponseMessage>> send,
        CancellationToken cancellationToken)
    {
        ApplyOfficerHeaders();
        var response = await send(cancellationToken);
        await ApiException.ThrowIfErrorAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken))!;
    }

    private async Task<string> SendStringAsync(
        Func<CancellationToken, Task<HttpResponseMessage>> send,
        CancellationToken cancellationToken)
    {
        ApplyOfficerHeaders();
        var response = await send(cancellationToken);
        await ApiException.ThrowIfErrorAsync(response, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
