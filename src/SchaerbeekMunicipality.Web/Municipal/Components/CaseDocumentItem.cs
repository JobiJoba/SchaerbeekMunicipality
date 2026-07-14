namespace SchaerbeekMunicipality.Web.Municipal.Components;

public sealed record CaseDocumentItem(
    Guid Id,
    string FileName,
    DateTimeOffset UploadedAt,
    string? TypeLabel = null);