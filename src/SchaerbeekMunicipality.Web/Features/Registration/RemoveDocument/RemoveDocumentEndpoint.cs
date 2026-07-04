using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RemoveDocument;

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
                new RegistrationCaseId(id),
                new AdministrativeDocumentId(documentId),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid registration transition");
        }
    }
}
