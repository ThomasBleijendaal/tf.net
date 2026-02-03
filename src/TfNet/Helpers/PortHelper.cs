using System.Net;
using System.Net.Sockets;

namespace TfNet.Helpers;

internal static class PortHelper
{
    public static int GetFreeTcpPort()
    {
        using var l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();

        return ((IPEndPoint)l.LocalEndpoint).Port;
    }
}

