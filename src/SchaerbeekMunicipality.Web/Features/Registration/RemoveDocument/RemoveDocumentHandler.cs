using Microsoft.Extensions.Logging;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RemoveDocument;

public sealed class RemoveDocumentHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    RegistrationResidenceEvaluator residenceEvaluator,
    ILogger<RemoveDocumentHandler> logger)
{
    public async Task<RemoveDocumentResponse> Handle(
        RegistrationCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RemoveDocument),
            cancellationToken);

        registrationCase.EnsureCanAttachDocuments();

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (document.RegistrationCaseId != caseId)
        {
            throw new InvalidRegistrationTransitionException(
                "The document does not belong to this registration case.");
        }

        documentRepository.Remove(document);
        await documentStorage.DeleteAsync(document.StoragePath, cancellationToken);

        var policyResult = await residenceEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Removed document {DocumentId} from case {CaseId}",
            documentId.Value,
            caseId.Value);

        return new RemoveDocumentResponse(
            registrationCase.Id.Value,
            documentId.Value,
            registrationCase.Checklist.LegalResidenceEstablished,
            policyResult.IsValid ? null : policyResult.FailureReason);
    }
}
