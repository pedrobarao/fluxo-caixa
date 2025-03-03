using FC.Consolidado.Domain.Entities;
using FC.Core.Data;

namespace FC.Consolidado.Domain.Repositories;

public interface ITransacaoRepository : IRepository<Transacao>
{
    void Add(Transacao transacao);
    IAsyncEnumerable<Transacao> ObterTransacoesPorData(DateOnly data);
}