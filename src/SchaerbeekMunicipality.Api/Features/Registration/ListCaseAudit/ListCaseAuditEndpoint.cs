using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.ListCaseAudit;

namespace SchaerbeekMunicipality.Api.Features.Registration.ListCaseAudit;

public static class ListCaseAuditEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ListCaseAuditHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new RegistrationCaseId(id), cancellationToken);
        return Results.Ok(result);
    }
}
