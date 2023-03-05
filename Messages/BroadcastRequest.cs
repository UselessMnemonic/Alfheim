using System.Text.Json.Serialization;

namespace Hermodr.Messages;

public class BroadcastRequest : Message
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    public BroadcastRequest(int sequence)
        : base(CommandOps.Broadcast, sequence) {}
}
