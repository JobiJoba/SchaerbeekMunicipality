using FluentValidation;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Web.Validation;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.RejectChangeOfAddress;

public static class RejectChangeOfAddressEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RejectChangeOfAddressRequest request,
        RejectChangeOfAddressHandler handler,
        IValidator<RejectChangeOfAddressRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationResults.ToProblemDetails(validation);
        }

        try
        {
            var result = await handler.Handle(new ChangeOfAddressCaseId(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
