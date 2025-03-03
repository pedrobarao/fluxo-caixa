using FC.Cache;
using FC.Consolidado.Application.Builders;
using FC.Consolidado.Application.Mappings;
using FC.Consolidado.Application.Outputs;
using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace FC.Consolidado.Application.Queries;

public class SaldoConsolidadoQuery : ISaldoConsolidadoQuery
{
    private readonly ICacheService _cache;
    private readonly ITransacaoRepository _repository;
    private readonly SaldoConsolidadoCacheKeyBuilder _saldoConsolidadoCacheKeyBuilder;

    public SaldoConsolidadoQuery(ITransacaoRepository repository, ICacheService cache,
        IOptions<RedisCacheOptions> options)
    {
        _repository = repository;
        _cache = cache;
        _saldoConsolidadoCacheKeyBuilder = new SaldoConsolidadoCacheKeyBuilder(options.Value.InstanceName!);
    }

    public async IAsyncEnumerable<SaldoConsolidadoOutput> ObterPorData(DateOnly data)
    {
        var saldoConsolidado = await _cache.GetAsync<SaldoConsolidado>(_saldoConsolidadoCacheKeyBuilder.BuildKey(data));

        if (saldoConsolidado != null)
        {
            yield return saldoConsolidado.ToOutput();
            yield break;
        }

        var saldoParcial = new SaldoConsolidado(data);

        await foreach (var transacao in _repository.ObterTransacoesPorData(data))
        {
            saldoParcial.AdicionarTransacao(transacao);
            yield return saldoParcial.ToOutput();
        }

        await _cache.SetAsync(
            _saldoConsolidadoCacheKeyBuilder.BuildKey(data),
            saldoParcial);
    }
}