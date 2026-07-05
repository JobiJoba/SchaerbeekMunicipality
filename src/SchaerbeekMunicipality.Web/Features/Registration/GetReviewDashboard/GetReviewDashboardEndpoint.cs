namespace SchaerbeekMunicipality.Web.Features.Registration.GetReviewDashboard;

public static class GetReviewDashboardEndpoint
{
    public static async Task<IResult> Handle(
        GetReviewDashboardHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(cancellationToken);
        return Results.Ok(result);
    }
}
