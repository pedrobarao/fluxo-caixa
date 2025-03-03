using FC.Cache;
using FC.Consolidado.Application.DTOs;
using FC.Consolidado.Application.Mappings;
using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Repositories;
using FC.Core.Communication;
using FC.Core.Mediator;

namespace FC.Consolidado.Application.Commands;

public class NovaTransacaoCommandHandler : IRequestHandler<NovaTransacaoCommand, Result<SaldoConsolidado>>
{
    private readonly ICacheService _cacheService;
    private const string DateFormat = "yyyy-MM-dd";
    private readonly ITransacaoRepository _transacaoRepository;

    public NovaTransacaoCommandHandler(
        ITransacaoRepository transacaoRepository,
        ICacheService cacheService)
    {
        _transacaoRepository = transacaoRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<SaldoConsolidado>> Handle(
        NovaTransacaoCommand request,
        CancellationToken cancellationToken)
    {
        var transacao = new Transacao(
            request.Id,
            request.Valor,
            request.Descricao!,
            request.Tipo,
            request.DataHora.ToUniversalTime());

        var saldoConsolidado = await ProcessarSaldoConsolidado(transacao);

        return Result.Success(saldoConsolidado);
    }

    private async Task<SaldoConsolidado> ProcessarSaldoConsolidado(Transacao transacao)
    {
        var data = DateOnly.FromDateTime(transacao.DataHora);

        var saldoAnterior = await RecuperarSaldoConsolidado(data.AddDays(-1));
        var saldoAtual = await RecuperarSaldoConsolidado(data);

        await AtualizarSaldoConsolidado(saldoAtual, saldoAnterior, transacao);

        return saldoAtual;
    }

    private async Task<SaldoConsolidado> RecuperarSaldoConsolidado(DateOnly data)
    {
        var chaveCache = GerarChaveCache(data);
        var saldoCache = await _cacheService.GetAsync<SaldoConsolidadoDto>(chaveCache);

        if (saldoCache != null)
            return saldoCache.ToEntity();

        var saldo = new SaldoConsolidado(data);

        var transacoes = await _transacaoRepository.ObterTransacoesPorData(data);

        foreach (var transacao in transacoes)
            saldo.AdicionarTransacao(transacao);

        await _cacheService.SetAsync(chaveCache, saldo);
        return saldo;
    }

    private async Task AtualizarSaldoConsolidado(
        SaldoConsolidado saldoAtual,
        SaldoConsolidado saldoAnterior,
        Transacao transacao)
    {
        saldoAtual.DefinirSaldoInicial(saldoAnterior.SaldoFinal);
        saldoAtual.AdicionarTransacao(transacao);
        _transacaoRepository.Add(transacao);
        await _transacaoRepository.UnitOfWork.Commit();
        await PersistirSaldoNoCache(saldoAtual);
    }

    private string GerarChaveCache(DateOnly data) => data.ToString(DateFormat);

    private async Task PersistirSaldoNoCache(SaldoConsolidado saldo)
    {
        var saldoDto = saldo.ToDto();
        var chaveCache = GerarChaveCache(saldoDto.Data);
        await _cacheService.SetAsync(chaveCache, saldoDto);
    }
}