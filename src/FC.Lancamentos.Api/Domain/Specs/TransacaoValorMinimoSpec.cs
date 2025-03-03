using FC.Core.Specifications;
using FC.Lancamentos.Api.Domain.Entities;

namespace FC.Lancamentos.Api.Domain.Specs;

public class TransacaoValorMinimoSpec : Specification<Transacao>
{
    private const decimal ValorMinimo = 0;

    public TransacaoValorMinimoSpec()
        : base($"O valor da operação deve ser maior que R$ {ValorMinimo}")
    {
    }

    public override bool IsSatisfiedBy(Transacao transacao)
    {
        return transacao.Valor > ValorMinimo;
    }
}