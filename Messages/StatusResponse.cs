using System.Text.Json.Serialization;

namespace Hermodr.Messages;

public class StatusResponse : CommandResponse
{
    [JsonPropertyName("worldName")]
    public string WorldName { get; set; }
    
    [JsonPropertyName("players")]
    public string[] Players { get; set; }

    public StatusResponse(int sequence)
        : base(CommandOps.Status, sequence) {}
}
