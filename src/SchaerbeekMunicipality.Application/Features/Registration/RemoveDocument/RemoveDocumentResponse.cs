namespace SchaerbeekMunicipality.Application.Features.Registration.RemoveDocument;

public sealed record RemoveDocumentResponse(
    Guid CaseId,
    Guid DocumentId,
    bool LegalResidenceEstablished,
    string? PolicyMessage);
