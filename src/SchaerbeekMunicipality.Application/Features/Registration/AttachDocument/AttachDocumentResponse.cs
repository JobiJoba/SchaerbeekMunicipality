using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Application.Features.Registration.AttachDocument;

public sealed record AttachDocumentResponse(
    Guid DocumentId,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);
