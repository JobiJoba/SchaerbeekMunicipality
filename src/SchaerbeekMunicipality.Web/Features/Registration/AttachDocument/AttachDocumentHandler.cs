using FluentValidation;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;

public sealed class AttachDocumentHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    RegistrationExceptionEvaluator exceptionEvaluator,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<AttachDocumentResponse> Handle(
        RegistrationCaseId caseId,
        DocumentType documentType,
        string fileName,
        Stream fileContent,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(AttachDocument),
            cancellationToken);

        registrationCase.EnsureCanAttachDocuments();

        var stored = await documentStorage.SaveAsync(fileContent, fileName, cancellationToken);

        var document = AdministrativeDocument.Create(
            caseId,
            documentType,
            fileName,
            stored.StoragePath,
            stored.ContentHash,
            OfficerId.From(currentOfficer.OfficerId),
            timeProvider.GetUtcNow());

        await documentRepository.AddAsync(document, cancellationToken);

        var existingDocuments = await documentRepository.ListByCaseIdAsync(caseId, cancellationToken);
        var documentTypes = existingDocuments
            .Select(d => d.DocumentType)
            .Append(documentType)
            .ToList();

        await exceptionEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken,
            documentTypesOverride: documentTypes);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new AttachDocumentResponse(
            document.Id.Value,
            document.DocumentType,
            document.FileName,
            document.UploadedAt);
    }
}
