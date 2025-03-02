using FC.Consolidado.Application.Outputs;
using FC.Consolidado.Domain.Entities;

namespace FC.Consolidado.Application.Queries;

public interface ISaldoConsolidadoQuery
{
    Task<SaldoConsolidadoOutput> ObterPorData(DateTime data);
}