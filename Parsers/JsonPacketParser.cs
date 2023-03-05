using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Hermodr.Gateway.Op;

namespace Hermodr.Parsers;

public class JsonPacketParser : PacketParser
{
    public override IAsyncEnumerable<Packet> DeserializeStream(Stream stream)
    {
        return JsonSerializer.DeserializeAsyncEnumerable<Packet>(stream);
    }

    public override Task Serialize(Stream stream, Packet message)
    {
        
    }
}
