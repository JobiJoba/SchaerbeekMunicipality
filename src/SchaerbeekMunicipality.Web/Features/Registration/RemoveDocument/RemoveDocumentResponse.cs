namespace SchaerbeekMunicipality.Web.Features.Registration.RemoveDocument;

public sealed record RemoveDocumentResponse(
    Guid CaseId,
    Guid DocumentId,
    bool LegalResidenceEstablished,
    string? PolicyMessage);
