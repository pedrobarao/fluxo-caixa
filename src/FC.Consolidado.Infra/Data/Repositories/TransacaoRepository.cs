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

    public async Task<IEnumerable<Transacao>> ObterTransacoesPorData(DateOnly data)
    {
        var dataHora = DateTime.SpecifyKind(
            data.ToDateTime(new TimeOnly(0, 0, 0)),
            DateTimeKind.Utc
        );

        return await context.Transacoes.Where(e => e.DataHora.Date == dataHora.Date).ToListAsync();
    }

    public void Dispose()
    {
        context?.Dispose();
    }
}