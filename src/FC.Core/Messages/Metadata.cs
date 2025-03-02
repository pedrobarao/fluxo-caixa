namespace FC.Core.Messages;

public record Metadata
{
    public Metadata()
    {
        MessageType = GetType().Name;
        Id = Guid.NewGuid();
        OccurredAt = DateTime.Now;
    }

    public string MessageType { get; init; }
    public Guid Id { get; init; }
    public DateTime OccurredAt { get; init; }
}