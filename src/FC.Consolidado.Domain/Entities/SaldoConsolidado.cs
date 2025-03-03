using FC.Core.DomainObjects;

namespace FC.Consolidado.Domain.Entities;

public class SaldoConsolidado
{
    public SaldoConsolidado(DateOnly data)
    {
        Data = data;
        SaldoInicial = 0;
        TotalCreditos = 0;
        TotalDebitos = 0;
        Transacoes = [];
    }

    public DateOnly Data { get; }
    public decimal SaldoInicial { get; private set; }
    public decimal SaldoFinal => TotalCreditos - TotalDebitos;
    public decimal TotalCreditos { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public List<Transacao> Transacoes { get; }

    public void AdicionarTransacao(Transacao transacao)
    {
        ValidarTransacao(transacao);

        Transacoes.Add(transacao);

        if (transacao.Tipo == TipoTransacao.Credito) TotalCreditos += transacao.Valor;
        else TotalDebitos += transacao.Valor;
    }

    public void DefinirSaldoInicial(decimal valor)
    {
        SaldoInicial = valor;
    }

    private void ValidarTransacao(Transacao transacao)
    {
        if (DateOnly.FromDateTime(transacao.DataHora) != Data)
            throw new DomainException("A data da transação deve ser igual a data informada para o saldo consolidado");
    }
}