namespace FC.Core.Mediator;

public interface IPipelineBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
} 