using FC.Consolidado.Application.Outputs;

namespace FC.Consolidado.Application.Queries;

public interface ISaldoConsolidadoQuery
{
    IAsyncEnumerable<SaldoConsolidadoOutput> ObterPorData(DateOnly data);
}