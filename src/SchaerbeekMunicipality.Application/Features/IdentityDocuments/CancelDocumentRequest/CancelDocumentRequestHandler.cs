using FluentValidation;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.CancelDocumentRequest;

public sealed record CancelDocumentRequestRequest(string Reason);

public sealed record CancelDocumentRequestResponse(
    Guid CaseId,
    DocumentRequestCaseStatus Status,
    string? CancellationReason,
    DateTimeOffset? CancelledAt);

public sealed class CancelDocumentRequestValidator : AbstractValidator<CancelDocumentRequestRequest>
{
    public CancelDocumentRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}

public sealed class CancelDocumentRequestHandler(
    DocumentRequestCaseGuard caseGuard,
    IDocumentRequestCaseRepository caseRepository,
    IValidator<CancelDocumentRequestRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<CancelDocumentRequestResponse> Handle(
        DocumentRequestCaseId caseId,
        CancelDocumentRequestRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var documentRequestCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(CancelDocumentRequest),
            cancellationToken);

        documentRequestCase.Cancel(request.Reason, timeProvider.GetUtcNow());
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new CancelDocumentRequestResponse(
            documentRequestCase.Id.Value,
            documentRequestCase.Status,
            documentRequestCase.CancellationReason,
            documentRequestCase.CancelledAt);
    }
}