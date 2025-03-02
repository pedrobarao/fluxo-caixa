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

    public async Task<IEnumerable<Transacao>> ObterTransacoesPorData(DateTime data)
    {
        return await context.Transacoes.Where(e => e.DataHora.Date == data.Date).ToListAsync();
    }

    public void Dispose()
    {
        context?.Dispose();
    }
}