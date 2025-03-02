using FC.Core.Communication;
using FC.Core.Mediator;
using FC.Lancamentos.Api.Domain.Entities;
using FC.Lancamentos.Api.Domain.Events;
using FC.MessageBus;

namespace FC.Lancamentos.Api.Application.Commands;

public class NovaTransacaoCommandHandler(IMessageBus bus) : IRequestHandler<NovaTransacaoCommand, Result>
{
    public async Task<Result> Handle(NovaTransacaoCommand request, CancellationToken cancellationToken)
    {
        var transacao = new Transacao(request.Valor, request.Descricao, request.Tipo);
        var validacaoTransacao = transacao.Validate();

        if (validacaoTransacao.IsInvalid)
            return Result.Failure(validacaoTransacao.Errors);

        await bus.Publish(LancamentoRealizadoEvent.Create(transacao), cancellationToken);
        
        return Result.Success();
    }
}