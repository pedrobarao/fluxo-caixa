namespace FC.Consolidado.Application.Responses;

public class TransacaoOutput
{
    public decimal Valor { get; set; }
    public string Descricao { get; set; }
    public string Tipo { get; set; }
    public DateTime DataHora { get; set; }
}