using FC.Consolidado.Application.DTOs;
using FC.Consolidado.Domain.Entities;

namespace FC.Consolidado.Application.Mappings;

public static class SaldoConsolidadoMapping
{
    public static SaldoConsolidadoDto ToDto(this SaldoConsolidado entity)
    {
        return new SaldoConsolidadoDto
        {
            Data = entity.Data,
            SaldoInicial = entity.SaldoInicial,
            SaldoFinal = entity.SaldoFinal,
            TotalCreditos = entity.TotalCreditos,
            TotalDebitos = entity.TotalDebitos,
            Transacoes = entity.Transacoes.Select(t => new TransacaoDto
            {
                Id = t.Id,
                Valor = t.Valor,
                Descricao = t.Descricao,
                Tipo = t.Tipo.ToString(),
                DataHora = t.DataHora
            })
        };
    }

    public static SaldoConsolidado ToEntity(this SaldoConsolidadoDto dto)
    {
        var entity = new SaldoConsolidado(dto.Data);
        entity.DefinirSaldoInicial(dto.SaldoInicial);

        foreach (var transacaoDto in dto.Transacoes)
        {
            entity.AdicionarTransacao(new Transacao(transacaoDto.Id,
                transacaoDto.Valor,
                transacaoDto.Descricao,
                Enum.Parse<TipoTransacao>(transacaoDto.Tipo),
                transacaoDto.DataHora));
        }

        return entity;
    }
}