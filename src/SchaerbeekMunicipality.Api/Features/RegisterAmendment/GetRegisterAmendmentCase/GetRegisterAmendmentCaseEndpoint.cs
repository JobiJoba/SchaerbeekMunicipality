using SchaerbeekMunicipality.Application.Features.RegisterAmendment.GetRegisterAmendmentCase;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.GetRegisterAmendmentCase;

public static class GetRegisterAmendmentCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetRegisterAmendmentCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var detail = await handler.Handle(RegisterAmendmentCaseId.From(id), cancellationToken);
        return detail is null ? Results.NotFound() : Results.Ok(detail);
    }
}
