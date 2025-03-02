namespace FC.Core.Mediator;

public interface IMediatorHandler
{
    Task<TResponse?> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}