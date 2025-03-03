namespace FC.Consolidado.Application.Outputs;

public class TransacaoOutput
{
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public DateTime DataHora { get; set; }
}