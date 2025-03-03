namespace FC.Core.Mediator;

public interface INotificationHandler<in TNotification>
    where TNotification : MediatR.INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}