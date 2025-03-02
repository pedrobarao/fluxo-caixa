using FC.Core.Messages;
using FC.Lancamentos.Api.Domain.Entities;

namespace FC.Lancamentos.Api.Domain.Events;

public record TransacaoCriadaEvent : Event
{
    public TransacaoCriadaEvent(Transacao transacao)
    {
        AggregateId = transacao.Id;
        Valor = transacao.Valor;
        Descricao = transacao.Descricao;
        Tipo = transacao.Tipo.ToString();
        DataHora = transacao.DataHora;
    }

    public decimal Valor { get; init; }
    public string Descricao { get; init; } = null!;
    public string Tipo { get; init; } = null!;
    public DateTime DataHora { get; init; }
}