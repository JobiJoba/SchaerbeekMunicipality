using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApproveRegisterAmendment;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.ApproveRegisterAmendment;

public static class ApproveRegisterAmendmentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ApproveRegisterAmendmentRequest request,
        ApproveRegisterAmendmentHandler handler,
        CancellationToken cancellationToken)
    {
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
