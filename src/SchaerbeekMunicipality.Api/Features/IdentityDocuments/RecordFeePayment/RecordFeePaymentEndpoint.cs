using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.RecordFeePayment;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.RecordFeePayment;

public static class RecordFeePaymentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RecordFeePaymentRequest request,
        RecordFeePaymentHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(
            DocumentRequestCaseId.From(id),
            request,
            cancellationToken);

        return Results.Ok(response);
    }
}
