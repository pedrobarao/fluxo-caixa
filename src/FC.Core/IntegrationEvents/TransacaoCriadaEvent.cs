using FC.Core.Messages;

namespace FC.Core.IntegrationEvents;

public class TransacaoCriadaEvent : Event
{
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
    public string? Tipo { get; set; }
    public DateTime DataHora { get; set; }
}