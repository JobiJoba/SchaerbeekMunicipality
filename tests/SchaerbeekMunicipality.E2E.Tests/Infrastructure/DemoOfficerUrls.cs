using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.E2E.Tests.Infrastructure;

internal static class DemoOfficerUrls
{
    internal const string QueryParameterName = "demoOfficer";

    internal static string WithOfficer(Uri baseUri, string path, Guid officerId)
    {
        var normalizedPath = path.StartsWith('/') ? path : $"/{path}";
        var uri = new Uri(baseUri, normalizedPath);
        var separator = string.IsNullOrEmpty(uri.Query) ? "?" : "&";
        return $"{uri}{separator}{QueryParameterName}={officerId}";
    }

    internal static string Reception(Uri baseUri, string path)
    {
        return WithOfficer(baseUri, path, CurrentOfficer.ReceptionOfficerId);
    }

    internal static string Population(Uri baseUri, string path)
    {
        return WithOfficer(baseUri, path, CurrentOfficer.PopulationOfficerId);
    }
}