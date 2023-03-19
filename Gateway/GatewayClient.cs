using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Hermodr.Extensions;
using Hermodr.Gateway.Packets;

namespace Hermodr.Gateway;

/// <summary>
/// A simple TCP wrapper that provides functions to transmit ZPackages to/from TCP clients.
/// </summary>
public class GatewayClient : IDisposable
{
    public readonly TcpClient Client;
    private readonly SemaphoreSlim _sendSlim = new(1, 1);

    public GatewayClient(TcpClient client)
    {
        Client = client;
    }

    private readonly byte[] _recvBuffer = new byte[12];
    public async Task<BinaryPacket> RecvAsync()
    {
        var stream = Client.GetStream();
        await stream.ReadExactAsync(_recvBuffer, 0, _recvBuffer.Length);
        var id = DataEncodings.GetIntBE(_recvBuffer, 0);
        var op = DataEncodings.GetIntBE(_recvBuffer, 4);
        var payloadSize = DataEncodings.GetIntBE(_recvBuffer, 8);

        if (payloadSize < 0)
            throw new InvalidDataException($"Payload size {payloadSize} is invalid");
        if (payloadSize > 0)
        {
            var payload = new byte[payloadSize];
            await stream.ReadExactAsync(payload, 0, payload.Length);
            return new BinaryPacket(id, op, payload);
        }
        return new BinaryPacket(id, op);
    }

    private readonly byte[] _sendBuffer = new byte[12];
    public async Task SendAsync(BinaryPacket binaryPacket)
    {
        await _sendSlim.WaitAsync();
        try
        {
            var stream = Client.GetStream();

            DataEncodings.PutBytesBE(binaryPacket.Id, _sendBuffer, 0);
            DataEncodings.PutBytesBE(binaryPacket.Op, _sendBuffer, 4);
            DataEncodings.PutBytesBE(binaryPacket.Payload.Length, _sendBuffer, 8);
            await stream.WriteAsync(_sendBuffer, 0, _sendBuffer.Length);
            if (binaryPacket.Payload.Length > 0)
            {
                await stream.WriteAsync(binaryPacket.Payload, 0, binaryPacket.Payload.Length);
            }
        }
        finally
        {
            _sendSlim.Release();
        }
    }

    public void Dispose() => Client.Dispose();
}
