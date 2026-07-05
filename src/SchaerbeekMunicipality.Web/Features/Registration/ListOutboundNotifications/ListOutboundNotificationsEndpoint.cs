using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ListOutboundNotifications;

public static class ListOutboundNotificationsEndpoint
{
    public static async Task<IResult> Handle(
        Guid? caseId,
        ListOutboundNotificationsHandler handler,
        CancellationToken cancellationToken)
    {
        var filterCaseId = caseId is { } id ? new RegistrationCaseId(id) : (RegistrationCaseId?)null;
        var result = await handler.Handle(filterCaseId, cancellationToken);
        return Results.Ok(result);
    }
}
