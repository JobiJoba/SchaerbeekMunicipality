using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

public static class GetRegistrationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetRegistrationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var detail = await handler.Handle(new RegistrationCaseId(id), cancellationToken);
        return detail is null ? Results.NotFound() : Results.Ok(detail);
    }
}
