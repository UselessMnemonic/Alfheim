using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hermodr.Gateway.Op;

namespace Hermodr.Parsers;

public abstract class PacketParser
{
    public abstract IAsyncEnumerable<Packet<dynamic>> DeserializeStream(Stream stream);
    public abstract Task Serialize(Stream stream, Packet<dynamic> message);
}
