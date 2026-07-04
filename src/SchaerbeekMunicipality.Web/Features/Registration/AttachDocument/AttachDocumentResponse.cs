using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;

public sealed record AttachDocumentResponse(
    Guid DocumentId,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);
