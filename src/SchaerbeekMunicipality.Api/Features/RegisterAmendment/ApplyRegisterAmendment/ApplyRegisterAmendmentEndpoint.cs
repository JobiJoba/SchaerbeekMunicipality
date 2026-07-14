using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApplyRegisterAmendment;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.ApplyRegisterAmendment;

public static class ApplyRegisterAmendmentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ApplyRegisterAmendmentHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(RegisterAmendmentCaseId.From(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidRegisterAmendmentTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
