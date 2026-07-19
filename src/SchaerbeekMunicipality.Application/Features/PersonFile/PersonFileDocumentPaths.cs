namespace SchaerbeekMunicipality.Application.Features.PersonFile;

internal static class PersonFileDocumentPaths
{
    public static string BuildDownloadPath(string workflow, Guid caseId, Guid documentId)
    {
        return workflow switch
        {
            "Registration" => $"/api/registration/cases/{caseId}/documents/{documentId}",
            "Birth declaration" => $"/api/birth-declarations/cases/{caseId}/documents/{documentId}",
            "Change of address" => $"/api/change-of-address/cases/{caseId}/documents/{documentId}",
            "Identity document" => $"/api/identity-documents/requests/{caseId}/documents/{documentId}",
            "Register amendment" => $"/api/register-amendments/cases/{caseId}/documents/{documentId}",
            _ => throw new InvalidOperationException($"Unsupported workflow '{workflow}' for document download.")
        };
    }
}
