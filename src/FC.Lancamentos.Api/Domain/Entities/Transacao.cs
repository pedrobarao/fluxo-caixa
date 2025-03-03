using FC.Core.Communication;
using FC.Core.DomainObjects;
using FC.Lancamentos.Api.Domain.Specs;

namespace FC.Lancamentos.Api.Domain.Entities;

public class Transacao : Entity, IAggregateRoot
{
    public Transacao(decimal valor, string descricao, TipoTransacao tipo)
    {
        Valor = valor;
        Descricao = descricao;
        Tipo = tipo;
        DataHora = DateTime.Now;
    }

    public decimal Valor { get; private set; }
    public string Descricao { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public DateTime DataHora { get; private set; }

    public ValidationResult Validate()
    {
        var spec = new TransacaoValorMinimoSpec()
            .And(new TransacaoDescricaoObrigatoriaSpec())
            .And(new TransacaoTipoValidoSpec());

        return spec.Validate(this);
    }
}