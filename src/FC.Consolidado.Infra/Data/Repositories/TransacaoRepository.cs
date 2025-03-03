using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Repositories;
using FC.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace FC.Consolidado.Infra.Data.Repositories;

public sealed class TransacaoRepository(ConsolidadoDbContext context) : ITransacaoRepository
{
    public IUnitOfWork UnitOfWork => context!;

    public void Add(Transacao transacao)
    {
        context.Transacoes.Add(transacao);
    }

    public async IAsyncEnumerable<Transacao> ObterTransacoesPorData(DateOnly data)
    {
        var dataHora = data.ToDateTime(new TimeOnly(0, 0, 0));

        var result = context.Transacoes.Where(e => e.DataHora.Date == dataHora.Date).AsAsyncEnumerable();

        await foreach (var transacao in result) yield return transacao;
    }

    public void Dispose()
    {
        context?.Dispose();
    }
}