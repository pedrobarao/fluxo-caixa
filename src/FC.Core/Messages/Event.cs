using FC.Core.Mediator;

namespace FC.Core.Messages;

public abstract class Event : Message, INotification
{
    public Guid Id { get; protected set; }
    public Guid AggregateId { get; protected set; }
    public Guid CorrelationId { get; protected set; }
    public DateTime OccurredAt { get; protected set; }
    public object Payload { get; protected set; }
}