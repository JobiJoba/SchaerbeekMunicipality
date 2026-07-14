using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ClaimRegisterAmendmentCase;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.ClaimRegisterAmendmentCase;

public static class ClaimRegisterAmendmentCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimRegisterAmendmentCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(RegisterAmendmentCaseId.From(id), cancellationToken);
        return Results.Ok(result);
    }
}

public static class AutoClaimRegisterAmendmentCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimRegisterAmendmentCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.TryAutoClaimAsync(RegisterAmendmentCaseId.From(id), cancellationToken);
        return result is null ? Results.NoContent() : Results.Ok(result);
    }
}
