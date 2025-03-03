using FC.Consolidado.Application.DTOs;

namespace FC.Consolidado.Application.Queries;

public interface ISaldoConsolidadoQuery
{
    Task<SaldoConsolidadoDto> ObterPorData(DateOnly data);
}