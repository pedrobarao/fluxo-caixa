namespace FC.Consolidado.Application.DTOs;

public class SaldoConsolidadoDto
{
    public DateOnly Data { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoFinal { get; set; }
    public decimal TotalCreditos { get; set; }
    public decimal TotalDebitos { get; set; }
    public IEnumerable<TransacaoDto> Transacoes { get; set; }
}