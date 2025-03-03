using FC.Core.DomainObjects;

namespace FC.Consolidado.Domain.Entities;

public class Transacao : Entity, IAggregateRoot
{
    protected Transacao()
    {
    }

    public Transacao(Guid id, decimal valor, string descricao, TipoTransacao tipo, DateTime dataHora)
    {
        Id = id;
        Valor = valor;
        Descricao = descricao;
        Tipo = tipo;
        DataHora = dataHora;
    }

    public decimal Valor { get; private set; }
    public string Descricao { get; private set; } = null!;
    public TipoTransacao Tipo { get; private set; }
    public DateTime DataHora { get; private set; }
}