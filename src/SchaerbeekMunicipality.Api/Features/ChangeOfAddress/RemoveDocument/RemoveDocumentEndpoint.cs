using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RemoveDocument;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.RemoveDocument;

public static class RemoveDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        Guid documentId,
        RemoveDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(
                new ChangeOfAddressCaseId(id),
                new AdministrativeDocumentId(documentId),
                cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
