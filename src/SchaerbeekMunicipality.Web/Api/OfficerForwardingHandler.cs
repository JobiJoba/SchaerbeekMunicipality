using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Web.Api;

public sealed class OfficerForwardingHandler(ICurrentOfficer currentOfficer) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Remove(DemoOfficerHeaders.OfficerId);
        request.Headers.Remove(DemoOfficerHeaders.OfficerRole);
        request.Headers.Remove(DemoOfficerHeaders.OfficerName);

        request.Headers.Add(DemoOfficerHeaders.OfficerId, currentOfficer.OfficerId.ToString());
        request.Headers.Add(DemoOfficerHeaders.OfficerRole, currentOfficer.Role.ToString());
        request.Headers.Add(DemoOfficerHeaders.OfficerName, currentOfficer.DisplayName);

        return base.SendAsync(request, cancellationToken);
    }
}
