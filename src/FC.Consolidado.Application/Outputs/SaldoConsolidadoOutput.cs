using FC.Consolidado.Application.Responses;

namespace FC.Consolidado.Application.Outputs;

public class SaldoConsolidadoOutput
{
    public DateTime Data { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoFinal { get; set; }
    public decimal TotalCreditos { get; set; }
    public decimal TotalDebitos { get; set; }
    public IEnumerable<TransacaoOutput> Transacoes { get; set; }
}