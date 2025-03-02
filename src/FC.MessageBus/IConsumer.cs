using FC.Core.Messages;

namespace FC.MessageBus;

public interface IConsumer<TEvent> : MassTransit.IConsumer<TEvent> where TEvent : Event
{
}