using System;
using System.Net;
using System.Net.Sockets;
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

    /// <summary>
    /// Accepts a pending connection request as an asynchronous operation.
    /// </summary>
    /// <returns>
    /// The task object representing the asynchronous operation. The Result property on the task object returns a GatewayClient used to send and receive data.
    /// </returns>
    /// <exception cref="InvalidOperationException">The listener has not been started with a call to Start().</exception>
    /// <exception cref="SocketException">Use the ErrorCode property to obtain the specific error code. When you have obtained this code, you can refer to the Windows Sockets version 2 API error code documentation in MSDN for a detailed description of the error.</exception>
    public async Task<GatewayClient> AcceptGatewayClientAsync()
    {
        var client = await Server.AcceptTcpClientAsync();
        return new GatewayClient(client);
    }
}
