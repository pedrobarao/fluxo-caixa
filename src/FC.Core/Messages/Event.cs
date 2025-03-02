using FC.Core.Mediator;

namespace FC.Core.Messages;

public abstract record Event : Message, INotification
{
}