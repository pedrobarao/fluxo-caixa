using System.ComponentModel;
using FC.Consolidado.Domain.Entities;
using FC.Core.DomainObjects;
using FluentAssertions;

namespace FC.Consolidado.Domain.Test.Entities;

public class SaldoConsolidadoTest
{
    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve inicializar corretamente um saldo consolidado")]
    public void CriarSaldoConsolidado_DeveInicializarCorretamente()
    {
        var data = DateOnly.FromDateTime(DateTime.Now);

        var saldo = new SaldoConsolidado(data);

        saldo.Data.Should().Be(data);
        saldo.SaldoInicial.Should().Be(0);
        saldo.TotalCreditos.Should().Be(0);
        saldo.TotalDebitos.Should().Be(0);
        saldo.SaldoFinal.Should().Be(0);
        saldo.Transacoes.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve definir saldo inicial corretamente")]
    public void DefinirSaldoInicial_DeveAtualizarSaldoInicial()
    {
        var data = DateOnly.FromDateTime(DateTime.Now);
        var saldo = new SaldoConsolidado(data);
        var valorSaldoInicial = 500m;

        saldo.DefinirSaldoInicial(valorSaldoInicial);

        saldo.SaldoInicial.Should().Be(valorSaldoInicial);
        saldo.SaldoFinal.Should().Be(valorSaldoInicial);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve adicionar transação de crédito corretamente")]
    public void AdicionarTransacao_Credito_DeveAtualizarTotalCreditosESaldoFinal()
    {
        var data = DateOnly.FromDateTime(DateTime.Now);
        var saldo = new SaldoConsolidado(data);
        var valorSaldoInicial = 500m;
        var valorCredito = 100m;

        saldo.DefinirSaldoInicial(valorSaldoInicial);

        var transacao = new Transacao(
            Guid.NewGuid(),
            valorCredito,
            "Transação de crédito",
            TipoTransacao.Credito,
            data.ToDateTime(TimeOnly.MinValue));

        saldo.AdicionarTransacao(transacao);

        saldo.TotalCreditos.Should().Be(valorCredito);
        saldo.TotalDebitos.Should().Be(0);
        saldo.SaldoFinal.Should().Be(valorSaldoInicial + valorCredito);
        saldo.Transacoes.Should().Contain(transacao);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve adicionar transação de débito corretamente")]
    public void AdicionarTransacao_Debito_DeveAtualizarTotalDebitosESaldoFinal()
    {
        var data = DateOnly.FromDateTime(DateTime.Now);
        var saldo = new SaldoConsolidado(data);
        var valorSaldoInicial = 500m;
        var valorDebito = 100m;

        saldo.DefinirSaldoInicial(valorSaldoInicial);

        var transacao = new Transacao(
            Guid.NewGuid(),
            valorDebito,
            "Transação de débito",
            TipoTransacao.Debito,
            data.ToDateTime(TimeOnly.MinValue));

        saldo.AdicionarTransacao(transacao);

        saldo.TotalCreditos.Should().Be(0);
        saldo.TotalDebitos.Should().Be(valorDebito);
        saldo.SaldoFinal.Should().Be(valorSaldoInicial - valorDebito);
        saldo.Transacoes.Should().Contain(transacao);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve adicionar múltiplas transações corretamente")]
    public void AdicionarTransacoes_Multiplas_DeveAtualizarTotaisESaldoFinal()
    {
        var data = DateOnly.FromDateTime(DateTime.Now);
        var saldo = new SaldoConsolidado(data);
        var valorSaldoInicial = 500m;
        var valorCredito1 = 100m;
        var valorCredito2 = 200m;
        var valorDebito1 = 150m;
        var valorDebito2 = 50m;

        saldo.DefinirSaldoInicial(valorSaldoInicial);

        var transacaoCredito1 = new Transacao(
            Guid.NewGuid(),
            valorCredito1,
            "Transação de crédito 1",
            TipoTransacao.Credito,
            data.ToDateTime(TimeOnly.MinValue));

        var transacaoCredito2 = new Transacao(
            Guid.NewGuid(),
            valorCredito2,
            "Transação de crédito 2",
            TipoTransacao.Credito,
            data.ToDateTime(TimeOnly.MinValue));

        var transacaoDebito1 = new Transacao(
            Guid.NewGuid(),
            valorDebito1,
            "Transação de débito 1",
            TipoTransacao.Debito,
            data.ToDateTime(TimeOnly.MinValue));

        var transacaoDebito2 = new Transacao(
            Guid.NewGuid(),
            valorDebito2,
            "Transação de débito 2",
            TipoTransacao.Debito,
            data.ToDateTime(TimeOnly.MinValue));

        saldo.AdicionarTransacao(transacaoCredito1);
        saldo.AdicionarTransacao(transacaoDebito1);
        saldo.AdicionarTransacao(transacaoCredito2);
        saldo.AdicionarTransacao(transacaoDebito2);

        var totalCreditos = valorCredito1 + valorCredito2;
        var totalDebitos = valorDebito1 + valorDebito2;
        var saldoFinalEsperado = valorSaldoInicial + totalCreditos - totalDebitos;

        saldo.TotalCreditos.Should().Be(totalCreditos);
        saldo.TotalDebitos.Should().Be(totalDebitos);
        saldo.SaldoFinal.Should().Be(saldoFinalEsperado);
        saldo.Transacoes.Should().HaveCount(4);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve lançar exceção ao adicionar transação com data diferente")]
    public void AdicionarTransacao_DataDiferente_DeveLancarExcecao()
    {
        var dataAtual = DateOnly.FromDateTime(DateTime.Now);
        var dataTransacao = dataAtual.AddDays(1);
        var saldo = new SaldoConsolidado(dataAtual);

        var transacao = new Transacao(
            Guid.NewGuid(),
            100m,
            "Transação com data diferente",
            TipoTransacao.Credito,
            dataTransacao.ToDateTime(TimeOnly.MinValue));

        var act = () => saldo.AdicionarTransacao(transacao);

        act.Should().Throw<DomainException>()
            .WithMessage("*data da transação*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve calcular saldo final corretamente após múltiplas operações")]
    public void SaldoFinal_MultiplaOperacoes_DeveCalcularCorretamente()
    {
        var data = DateOnly.FromDateTime(DateTime.Now);
        var saldo = new SaldoConsolidado(data);

        saldo.DefinirSaldoInicial(1000m);
        saldo.SaldoFinal.Should().Be(1000m);

        saldo.AdicionarTransacao(new Transacao(
            Guid.NewGuid(),
            500m,
            "Crédito",
            TipoTransacao.Credito,
            data.ToDateTime(TimeOnly.MinValue)));
        saldo.SaldoFinal.Should().Be(1500m);

        saldo.AdicionarTransacao(new Transacao(
            Guid.NewGuid(),
            300m,
            "Débito",
            TipoTransacao.Debito,
            data.ToDateTime(TimeOnly.MinValue)));
        saldo.SaldoFinal.Should().Be(1200m);

        saldo.DefinirSaldoInicial(2000m);
        saldo.SaldoFinal.Should().Be(2200m);
    }
}