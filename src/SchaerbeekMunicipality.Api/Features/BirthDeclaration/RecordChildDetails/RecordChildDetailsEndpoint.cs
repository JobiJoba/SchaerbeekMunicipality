using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.RecordChildDetails;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.RecordChildDetails;

public static class RecordChildDetailsEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RecordChildDetailsRequest request,
        RecordChildDetailsHandler handler,
        IValidator<RecordChildDetailsRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        try
        {
            var result = await handler.Handle(new BirthDeclarationCaseId(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidBirthDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}