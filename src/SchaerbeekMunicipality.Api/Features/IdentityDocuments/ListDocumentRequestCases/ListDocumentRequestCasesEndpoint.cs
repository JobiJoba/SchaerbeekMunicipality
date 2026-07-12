using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ListDocumentRequestCases;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.ListDocumentRequestCases;

public static class ListDocumentRequestCasesEndpoint
{
    public static async Task<IResult> Handle(
        ListDocumentRequestCasesHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(cancellationToken);
        return Results.Ok(response);
    }
}
