using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace SchaerbeekMunicipality.Application.Auth;

internal static class DemoOfficerPersistence
{
    internal const string QueryParameterName = "demoOfficer";

    internal static bool TryGetOfficerId(NavigationManager navigation, out Guid officerId)
    {
        officerId = default;
        var uri = navigation.ToAbsoluteUri(navigation.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        if (!query.TryGetValue(QueryParameterName, out var rawValue) ||
            !Guid.TryParse(rawValue.ToString(), out var parsedId))
            return false;

        if (!DemoOfficers.All.Any(o => o.Id == parsedId)) return false;

        officerId = parsedId;
        return true;
    }

    internal static string AppendToUri(string uri, Guid officerId)
    {
        var parsed = new Uri(uri, UriKind.RelativeOrAbsolute);

        string path;
        Dictionary<string, string> query;

        if (parsed.IsAbsoluteUri)
        {
            path = parsed.GetLeftPart(UriPartial.Path);
            query = QueryHelpers.ParseQuery(parsed.Query)
                .ToDictionary(k => k.Key, k => k.Value.ToString());
        }
        else
        {
            var queryIndex = uri.IndexOf('?', StringComparison.Ordinal);
            path = queryIndex >= 0 ? uri[..queryIndex] : uri;
            var existingQuery = queryIndex >= 0 ? uri[(queryIndex + 1)..] : string.Empty;
            query = QueryHelpers.ParseQuery(existingQuery)
                .ToDictionary(k => k.Key, k => k.Value.ToString());
        }

        query[QueryParameterName] = officerId.ToString();

        var queryString = string.Join(
            "&",
            query.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return string.IsNullOrEmpty(queryString) ? path : $"{path}?{queryString}";
    }
}