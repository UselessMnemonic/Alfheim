using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Hermodr.Gateway.Op;

/// <summary>
/// A basic data packet.
/// </summary>
public class Packet
{ 
    [JsonPropertyName("op")]
    [JsonRequired]
    public int Op { get; }
    
    [JsonPropertyName("seq")]
    [JsonRequired]
    public int Sequence { get; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; }

    [JsonPropertyName("data")]
    [JsonRequired]
    public object Data { get; }
}
