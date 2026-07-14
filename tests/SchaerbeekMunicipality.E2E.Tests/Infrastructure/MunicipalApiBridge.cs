using SchaerbeekMunicipality.Web.Api;

namespace SchaerbeekMunicipality.E2E.Tests.Infrastructure;

internal sealed class MunicipalApiBridge : IMunicipalApiBridge
{
    private readonly HttpMessageHandler _handler;

    public MunicipalApiBridge(Uri baseAddress, HttpMessageHandler handler)
    {
        BaseAddress = baseAddress;
        _handler = handler;
    }

    public Uri BaseAddress { get; }

    public HttpMessageHandler CreateHandler()
    {
        return _handler;
    }
}