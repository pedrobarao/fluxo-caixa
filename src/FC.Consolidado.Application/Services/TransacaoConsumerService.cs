using FC.Consolidado.Application.Commands;
using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Events;
using FC.Core.Mediator;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FC.Consolidado.Application.Services;

public class TransacaoConsumerService : MessageBus.IConsumer<TransacaoCriadaEvent>
{
    private readonly ILogger<TransacaoConsumerService> _logger;
    private readonly IMediatorHandler _mediator;

    public TransacaoConsumerService(IMediatorHandler mediator, ILogger<TransacaoConsumerService> logger)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<TransacaoCriadaEvent> context)
    {
        try
        {
            _logger.LogInformation("Mensagem recebida: {Message}", context.Message);
            await _mediator.Send(GetCommand(context.Message));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Erro ao processar a mensagem: {Message}", ex.Message);
            throw;
        }
    }

    private static NovaTransacaoCommand GetCommand(TransacaoCriadaEvent message)
    {
        if (!Enum.TryParse(message.Tipo, out TipoTransacao tipo))
            throw new InvalidCastException("Não foi possível converter o tipo da transação");

        return new NovaTransacaoCommand
        {
            Id = message.AggregateId,
            Valor = message.Valor,
            Descricao = message.Descricao,
            Tipo = tipo,
            DataHora = message.DataHora
        };
    }
}