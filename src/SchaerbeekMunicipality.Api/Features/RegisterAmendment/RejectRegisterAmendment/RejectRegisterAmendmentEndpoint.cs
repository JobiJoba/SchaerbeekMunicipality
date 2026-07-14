using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.RejectRegisterAmendment;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.RejectRegisterAmendment;

public static class RejectRegisterAmendmentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RejectRegisterAmendmentRequest request,
        RejectRegisterAmendmentHandler handler,
        IValidator<RejectRegisterAmendmentRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        try
        {
            var result = await handler.Handle(RegisterAmendmentCaseId.From(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidRegisterAmendmentTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
