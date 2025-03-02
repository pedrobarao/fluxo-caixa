namespace FC.Core.Messages;

public abstract record Message
{
    protected Message()
    {
        Metadata = new Metadata();
    }

    public Guid AggregateId { get; protected set; }
    public Guid CorrelationId { get; protected set; }
    public Metadata Metadata { get; init; }
}