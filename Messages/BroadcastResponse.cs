using System.Text.Json.Serialization;

namespace Hermodr.Messages;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "op")]
[JsonDerivedType(typeof(StatusResponse), CommandOps.Status)]
[JsonDerivedType(typeof(BroadcastResponse), CommandOps.Broadcast)]
public class BroadcastResponse : CommandResponse
{
    public BroadcastResponse(int sequence)
        : base(CommandOps.Broadcast, sequence) {}
}
