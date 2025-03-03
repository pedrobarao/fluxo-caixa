using FC.Core.Communication;
using FC.Core.IntegrationEvents;
using FC.Core.Mediator;
using FC.Lancamentos.Api.Domain.Entities;
using MassTransit;

namespace FC.Lancamentos.Api.Application.Commands;

public class NovaTransacaoCommandHandler(IBus bus) : IRequestHandler<NovaTransacaoCommand, Result>
{
    public async Task<Result> Handle(NovaTransacaoCommand request, CancellationToken cancellationToken)
    {
        var transacao = new Transacao(request.Valor, request.Descricao, request.Tipo);
        var validacaoTransacao = transacao.Validate();

        if (validacaoTransacao.IsInvalid)
            return Result.Failure(validacaoTransacao.Errors);

        await bus.Publish(new TransacaoCriadaEvent
        {
            AggregateId = transacao.Id,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo.ToString(),
            DataHora = transacao.DataHora
        }, cancellationToken);

        return Result.Success();
    }
}