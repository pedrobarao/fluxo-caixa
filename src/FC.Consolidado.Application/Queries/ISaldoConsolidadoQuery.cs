using FC.Consolidado.Application.Outputs;

namespace FC.Consolidado.Application.Queries;

public interface ISaldoConsolidadoQuery
{
    Task<SaldoConsolidadoOutput> ObterPorData(DateOnly data);
}