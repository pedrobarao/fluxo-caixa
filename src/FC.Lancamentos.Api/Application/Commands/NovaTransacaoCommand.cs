using System.Text.Json.Serialization;
using FC.Core.Communication;
using FC.Core.Mediator;
using FC.Lancamentos.Api.Domain.Entities;

namespace FC.Lancamentos.Api.Application.Commands;

public class NovaTransacaoCommand : IRequest<Result>
{
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = null!;

    public TipoTransacao Tipo { get; set; }
}