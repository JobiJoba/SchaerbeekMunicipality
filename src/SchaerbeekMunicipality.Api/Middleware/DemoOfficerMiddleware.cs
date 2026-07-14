using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Api.Middleware;

public sealed class DemoOfficerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentOfficer currentOfficer)
    {
        if (context.Request.Headers.TryGetValue(DemoOfficerHeaders.OfficerId, out var officerIdHeader) &&
            Guid.TryParse(officerIdHeader.ToString(), out var officerId) &&
            context.Request.Headers.TryGetValue(DemoOfficerHeaders.OfficerRole, out var roleHeader) &&
            Enum.TryParse<OfficerRole>(roleHeader.ToString(), true, out var role))
        {
            var displayName = context.Request.Headers.TryGetValue(DemoOfficerHeaders.OfficerName, out var nameHeader)
                              && !string.IsNullOrWhiteSpace(nameHeader.ToString())
                ? nameHeader.ToString()
                : DemoOfficers.Find(officerId).DisplayName;

            currentOfficer.Impersonate(officerId, role, displayName);
        }

        await next(context);
    }
}

public static class DemoOfficerMiddlewareExtensions
{
    public static IApplicationBuilder UseDemoOfficerResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<DemoOfficerMiddleware>();
    }
}