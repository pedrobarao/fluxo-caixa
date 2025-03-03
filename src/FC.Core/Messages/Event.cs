namespace FC.Core.Messages;

public abstract class Event
{
    public Guid AggregateId { get; set; }
}