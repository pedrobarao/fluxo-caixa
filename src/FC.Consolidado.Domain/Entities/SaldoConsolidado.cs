using FC.Core.DomainObjects;

namespace FC.Consolidado.Domain.Entities;

public class SaldoConsolidado
{
    private readonly List<Transacao> _transacoes;

    public SaldoConsolidado(DateTime data, IEnumerable<Transacao> transacoes)
    {
        Data = data;
        SaldoInicial = 0;
        TotalCreditos = 0;
        TotalDebitos = 0;
        _transacoes = [];
        AdicionarTransacoes(transacoes);
    }

    public DateTime Data { get; }
    public decimal SaldoInicial { get; private set; }
    public decimal SaldoFinal => TotalCreditos - TotalDebitos;
    public decimal TotalCreditos { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public IReadOnlyCollection<Transacao> Transacoes => _transacoes;


    public void AdicionarTransacao(Transacao transacao)
    {
        ValidarTransacao(transacao);

        _transacoes.Add(transacao);

        if (transacao.Tipo == TipoTransacao.Credito) TotalCreditos += transacao.Valor;
        else TotalDebitos += transacao.Valor;
    }

    public void AdicionarTransacoes(IEnumerable<Transacao> transacoes)
    {
        foreach (var transacao in transacoes) AdicionarTransacao(transacao);
    }

    public void DefinirSaldoInicial(decimal valor)
    {
        SaldoInicial = valor;
    }

    private void ValidarTransacao(Transacao transacao)
    {
        if (transacao.DataHora.Date != Data)
            throw new DomainException("A data da transação deve ser igual a data informada para o saldo consolidado");
    }
}