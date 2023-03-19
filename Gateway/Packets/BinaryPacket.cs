using System;

namespace Hermodr.Gateway.Packets;

/// <summary>
/// A basic data packet.
/// </summary>
public struct BinaryPacket
{
    public readonly int Id;
    public readonly int Op;
    public readonly byte[] Payload;

    public BinaryPacket(int id, int op, byte[] payload)
    {
        if (payload == null)
            throw new ArgumentNullException(nameof(payload), "Payload cannot be null");
        Id = id;
        Op = op;
        Payload = payload;
    }

    public BinaryPacket(int id, int op) : this(id, op, Array.Empty<byte>()) {}
}
