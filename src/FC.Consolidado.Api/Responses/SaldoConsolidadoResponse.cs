namespace FC.Consolidado.Api.Responses;

public class SaldoConsolidadoResponse
{
    public decimal SaldoInicial { get; set; }
    public decimal SaldoFinal { get; set; }
    public IEnumerable<OperacaoResponse> Operacoes { get; set; }
}