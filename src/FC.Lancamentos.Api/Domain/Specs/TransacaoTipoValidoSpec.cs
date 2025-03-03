﻿using FC.Core.Specifications;
using FC.Lancamentos.Api.Domain.Entities;

namespace FC.Lancamentos.Api.Domain.Specs;

public class TransacaoTipoValidoSpec : Specification<Transacao>
{
    public TransacaoTipoValidoSpec() : base("Tipo da operação inválido")
    {
    }

    public override bool IsSatisfiedBy(Transacao transacao)
    {
        return Enum.IsDefined(typeof(TipoTransacao), transacao.Tipo);
    }
}