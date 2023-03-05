namespace Hermodr.Messages;

public class StatusRequest : Message
{
    public StatusRequest(int sequence)
        : base(CommandOps.Status, sequence) {}
}
