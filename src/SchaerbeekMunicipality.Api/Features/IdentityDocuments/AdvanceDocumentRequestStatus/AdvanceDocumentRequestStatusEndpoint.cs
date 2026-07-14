using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AdvanceDocumentRequestStatus;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.AdvanceDocumentRequestStatus;

public static class AdvanceDocumentRequestStatusEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        AdvanceDocumentRequestStatusHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(DocumentRequestCaseId.From(id), cancellationToken);
        return Results.Ok(response);
    }
}