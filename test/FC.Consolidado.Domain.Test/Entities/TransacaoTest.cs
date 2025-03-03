using System.ComponentModel;
using FC.Consolidado.Domain.Entities;
using FluentAssertions;

namespace FC.Consolidado.Domain.Test.Entities;

public class TransacaoTest
{
    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve inicializar corretamente uma transação")]
    public void CriarTransacao_DeveInicializarCorretamente()
    {
        var id = Guid.NewGuid();
        var valor = 100.00m;
        var descricao = "Pagamento de conta";
        var tipo = TipoTransacao.Debito;
        var dataHora = DateTime.Now;

        var transacao = new Transacao(id, valor, descricao, tipo, dataHora);

        transacao.Id.Should().Be(id);
        transacao.Valor.Should().Be(valor);
        transacao.Descricao.Should().Be(descricao);
        transacao.Tipo.Should().Be(tipo);
        transacao.DataHora.Should().Be(dataHora);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve criar transação de crédito corretamente")]
    public void CriarTransacao_Credito_DeveInicializarCorretamente()
    {
        var id = Guid.NewGuid();
        var valor = 100.00m;
        var descricao = "Depósito";
        var tipo = TipoTransacao.Credito;
        var dataHora = DateTime.Now;

        var transacao = new Transacao(id, valor, descricao, tipo, dataHora);

        transacao.Id.Should().Be(id);
        transacao.Valor.Should().Be(valor);
        transacao.Descricao.Should().Be(descricao);
        transacao.Tipo.Should().Be(TipoTransacao.Credito);
        transacao.DataHora.Should().Be(dataHora);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve criar transação de débito corretamente")]
    public void CriarTransacao_Debito_DeveInicializarCorretamente()
    {
        var id = Guid.NewGuid();
        var valor = 100.00m;
        var descricao = "Pagamento";
        var tipo = TipoTransacao.Debito;
        var dataHora = DateTime.Now;

        var transacao = new Transacao(id, valor, descricao, tipo, dataHora);

        transacao.Id.Should().Be(id);
        transacao.Valor.Should().Be(valor);
        transacao.Descricao.Should().Be(descricao);
        transacao.Tipo.Should().Be(TipoTransacao.Debito);
        transacao.DataHora.Should().Be(dataHora);
    }
}