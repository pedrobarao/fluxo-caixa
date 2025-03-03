using FC.Cache;
using FC.Consolidado.Application.DTOs;
using FC.Consolidado.Application.Mappings;
using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Repositories;

namespace FC.Consolidado.Application.Queries;

public class SaldoConsolidadoQuery : ISaldoConsolidadoQuery
{
    private readonly ICacheService _cache;
    private readonly ITransacaoRepository _repository;
    private const string DateFormat = "yyyy-MM-dd";

    public SaldoConsolidadoQuery(ITransacaoRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<SaldoConsolidadoDto> ObterPorData(DateOnly data)
    {
        var saldoConsolidado = await _cache.GetAsync<SaldoConsolidadoDto>(GerarChaveCache(data));

        if (saldoConsolidado != null) return saldoConsolidado;

        var saldoParcial = new SaldoConsolidado(data);
        var transacoes = await _repository.ObterTransacoesPorData(data);

        foreach (var transacao in transacoes) saldoParcial.AdicionarTransacao(transacao);

        var saldoDto = saldoParcial.ToDto();

        await _cache.SetAsync(
            GerarChaveCache(data),
            saldoDto);

        return saldoDto;
    }

    private string GerarChaveCache(DateOnly data) => data.ToString(DateFormat);
}