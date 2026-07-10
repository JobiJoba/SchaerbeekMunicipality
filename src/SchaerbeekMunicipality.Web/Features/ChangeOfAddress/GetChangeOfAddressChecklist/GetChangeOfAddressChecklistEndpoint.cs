using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.GetChangeOfAddressChecklist;

public static class GetChangeOfAddressChecklistEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetChangeOfAddressChecklistHandler handler,
        CancellationToken cancellationToken)
    {
        var checklist = await handler.Handle(new ChangeOfAddressCaseId(id), cancellationToken);
        return checklist is null ? Results.NotFound() : Results.Ok(checklist);
    }
}
