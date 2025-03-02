namespace FC.Core.Mediator;

public interface IRequestHandler<in TRequest, TResponse> : MediatR.IRequestHandler<TRequest, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
{
}