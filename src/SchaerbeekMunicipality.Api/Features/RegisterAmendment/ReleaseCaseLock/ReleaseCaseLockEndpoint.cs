using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ReleaseCaseLock;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.ReleaseCaseLock;

public static class ReleaseCaseLockEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ReleaseCaseLockHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(RegisterAmendmentCaseId.From(id), cancellationToken);
        return Results.Ok(result);
    }
}
