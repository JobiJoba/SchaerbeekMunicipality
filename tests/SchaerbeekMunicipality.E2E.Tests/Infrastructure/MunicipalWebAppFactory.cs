using System.Net;
using System.Net.Sockets;

namespace SchaerbeekMunicipality.E2E.Tests.Infrastructure;

internal static class MunicipalWebAppFactory
{
    internal static int GetFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
