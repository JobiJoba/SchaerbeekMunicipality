using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.RecordFeePayment;

public sealed record RecordFeePaymentRequest(string? PaymentReference);

public sealed record RecordFeePaymentResponse(
    Guid CaseId,
    bool FeePaid,
    string? FeePaymentReference);

public sealed class RecordFeePaymentHandler(
    DocumentRequestCaseGuard caseGuard,
    IDocumentRequestCaseRepository caseRepository)
{
    public async Task<RecordFeePaymentResponse> Handle(
        DocumentRequestCaseId caseId,
        RecordFeePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var documentRequestCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordFeePayment),
            cancellationToken);

        documentRequestCase.RecordFeePayment(request.PaymentReference);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordFeePaymentResponse(
            documentRequestCase.Id.Value,
            documentRequestCase.FeePaid,
            documentRequestCase.FeePaymentReference);
    }
}
