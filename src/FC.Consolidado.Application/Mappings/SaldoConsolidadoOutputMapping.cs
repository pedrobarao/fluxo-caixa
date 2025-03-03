using FC.Consolidado.Application.Outputs;
using FC.Consolidado.Domain.Entities;

namespace FC.Consolidado.Application.Mappings;

public static class SaldoConsolidadoOutputMapping
{
    public static SaldoConsolidadoOutput ToOutput(this SaldoConsolidado saldoConsolidado)
    {
        return new SaldoConsolidadoOutput
        {
            Data = saldoConsolidado.Data,
            SaldoInicial = saldoConsolidado.SaldoInicial,
            SaldoFinal = saldoConsolidado.SaldoFinal,
            TotalCreditos = saldoConsolidado.TotalCreditos,
            TotalDebitos = saldoConsolidado.TotalDebitos,
            Transacoes = saldoConsolidado.Transacoes.Select(t => new TransacaoOutput
            {
                Valor = t.Valor,
                Descricao = t.Descricao,
                Tipo = t.Tipo.ToString(),
                DataHora = t.DataHora
            })
        };
    }
}