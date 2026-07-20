using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.RecordDeathFacts;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.RecordDeathFacts;

public static class RecordDeathFactsEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RecordDeathFactsRequest request,
        RecordDeathFactsHandler handler,
        IValidator<RecordDeathFactsRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        try
        {
            var result = await handler.Handle(new DeathDeclarationCaseId(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidDeathDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
