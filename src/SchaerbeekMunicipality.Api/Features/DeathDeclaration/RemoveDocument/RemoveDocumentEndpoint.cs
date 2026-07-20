using SchaerbeekMunicipality.Application.Features.DeathDeclaration.RemoveDocument;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.RemoveDocument;

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
                new DeathDeclarationCaseId(id),
                new AdministrativeDocumentId(documentId),
                cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidDeathDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
