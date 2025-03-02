using FC.Core.Specifications;
using FC.Lancamentos.Api.Domain.Entities;

namespace FC.Lancamentos.Api.Domain.Specs;

public class OperacaoDescricaoObrigatoriaSpec : Specification<Transacao>
{
    public OperacaoDescricaoObrigatoriaSpec()
        : base("A descrição da operação deve ser informada")
    {
    }

    public override bool IsSatisfiedBy(Transacao transacao)
    {
        return !string.IsNullOrEmpty(transacao.Descricao);
    }
}