using FluentValidation;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.CancelDocumentRequest;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.CancelDocumentRequest;

public static class CancelDocumentRequestEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        CancelDocumentRequestRequest request,
        CancelDocumentRequestHandler handler,
        IValidator<CancelDocumentRequestRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var response = await handler.Handle(
            DocumentRequestCaseId.From(id),
            request,
            cancellationToken);

        return Results.Ok(response);
    }
}