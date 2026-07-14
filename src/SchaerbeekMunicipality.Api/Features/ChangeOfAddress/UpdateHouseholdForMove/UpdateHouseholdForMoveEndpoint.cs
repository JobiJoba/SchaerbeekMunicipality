using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.UpdateHouseholdForMove;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.UpdateHouseholdForMove;

public static class UpdateHouseholdForMoveEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        UpdateHouseholdForMoveRequest request,
        UpdateHouseholdForMoveHandler handler,
        IValidator<UpdateHouseholdForMoveRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

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