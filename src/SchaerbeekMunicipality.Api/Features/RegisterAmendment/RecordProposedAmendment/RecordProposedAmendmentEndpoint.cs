using SchaerbeekMunicipality.Application.Features.RegisterAmendment.RecordProposedAmendment;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.RecordProposedAmendment;

public static class RecordProposedAmendmentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RecordProposedAmendmentRequest request,
        RecordProposedAmendmentHandler handler,
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
