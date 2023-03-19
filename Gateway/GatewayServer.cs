using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hermodr.Gateway;

public class GatewayServer
{
    public TcpListener Server { get; }

    public GatewayServer(IPEndPoint serverEndpoint)
    {
        Server = new TcpListener(serverEndpoint);
    }

    public void Start()
    {
        Server.Start();
    }

    public void Stop()
    {
        Server.Stop();
    }

    public async Task<GatewayClient> AcceptGatewayClientAsync()
    {
        var client = await Server.AcceptTcpClientAsync();
        return new GatewayClient(client);
    }
}
