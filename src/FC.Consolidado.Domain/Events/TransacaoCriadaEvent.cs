using FC.Core.Messages;

namespace FC.Consolidado.Domain.Events;

public record TransacaoCriadaEvent : Event
{
    public decimal Valor { get; init; }
    public string Descricao { get; init; } = null!;
    public string Tipo { get; init; } = null!;
    public DateTime DataHora { get; init; }
}