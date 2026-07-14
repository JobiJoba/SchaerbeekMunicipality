using SchaerbeekMunicipality.Application.Features.RegisterAmendment.RemoveDocument;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.RemoveDocument;

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
            await handler.Handle(
                RegisterAmendmentCaseId.From(id),
                new AdministrativeDocumentId(documentId),
                cancellationToken);

            return Results.Ok(new { caseId = id, documentId });
        }
        catch (InvalidRegisterAmendmentTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
