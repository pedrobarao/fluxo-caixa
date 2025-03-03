namespace FC.Consolidado.Application.DTOs;

public class TransacaoDto
{
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public DateTime DataHora { get; set; }
}