using FC.Core.DomainObjects;

namespace FC.Consolidado.Domain.Entities;

public class Transacao : Entity, IAggregateRoot
{
    public decimal Valor { get; private set; }
    public string Descricao { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public DateTime Data { get; private set; }
}