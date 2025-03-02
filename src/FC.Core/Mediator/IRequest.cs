namespace FC.Core.Mediator;

public interface IRequest<out TResponse> : MediatR.IRequest<TResponse>
{
}