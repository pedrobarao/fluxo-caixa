using FC.Core.Messages;

namespace FC.MessageBus;

public interface IMessageBus
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : Event;
}