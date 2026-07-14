using FluentValidation;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.OpenDocumentRequestCase;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.OpenDocumentRequestCase;

public static class OpenDocumentRequestCaseEndpoint
{
    public static async Task<IResult> Handle(
        OpenDocumentRequestCaseRequest request,
        OpenDocumentRequestCaseHandler handler,
        IValidator<OpenDocumentRequestCaseRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        var response = await handler.Handle(request, cancellationToken);
        return Results.Created($"/api/identity-documents/requests/{response.CaseId}", response);
    }
}