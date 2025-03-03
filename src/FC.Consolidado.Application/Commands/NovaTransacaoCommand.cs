using FC.Consolidado.Domain.Entities;
using FC.Core.Communication;
using FC.Core.Mediator;

namespace FC.Consolidado.Application.Commands;

public class NovaTransacaoCommand : IRequest<Result<SaldoConsolidado>>
{
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
    public TipoTransacao Tipo { get; set; }
    public DateTime DataHora { get; set; }
}