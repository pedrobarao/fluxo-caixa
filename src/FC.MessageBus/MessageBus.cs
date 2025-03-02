using MassTransit;
using Event = FC.Core.Messages.Event;

namespace FC.MessageBus;

public class MessageBus(IBus bus) : IMessageBus
{
    public async Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : Event
    {
        await bus.Publish(message, cancellationToken);
    }
}