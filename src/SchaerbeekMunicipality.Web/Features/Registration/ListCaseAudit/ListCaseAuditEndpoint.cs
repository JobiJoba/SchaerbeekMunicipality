using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ListCaseAudit;

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
