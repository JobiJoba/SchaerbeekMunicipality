using SchaerbeekMunicipality.Application.Features.Registration.ListPendingPoliceVerifications;

namespace SchaerbeekMunicipality.Api.Features.Registration.ListPendingPoliceVerifications;

public static class ListPendingPoliceVerificationsEndpoint
{
    public static async Task<IResult> Handle(
        ListPendingPoliceVerificationsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(cancellationToken);
        return Results.Ok(result);
    }
}