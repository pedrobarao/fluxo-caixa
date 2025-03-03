using System.ComponentModel;
using FC.Lancamentos.Api.Domain.Entities;
using FluentAssertions;

namespace FC.Lancamentos.Api.Test.Domain.Entities;

public class TransacaoTest
{
    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve inicializar corretamente uma transação com parâmetros válidos")]
    public void CriarTransacao_ParametrosValidos_DeveInicializarCorretamente()
    {
        // Arrange
        var valor = 100.00m;
        var descricao = "Pagamento de conta";
        var tipo = TipoTransacao.Debito;

        // Act
        var transacao = new Transacao(valor, descricao, tipo);

        // Assert
        transacao.Valor.Should().Be(valor, "o valor deve ser inicializado com o valor informado no construtor");
        transacao.Descricao.Should()
            .Be(descricao, "a descrição deve ser inicializada com o texto informado no construtor");
        transacao.Tipo.Should().Be(tipo, "o tipo deve ser inicializado com o valor informado no construtor");
        transacao.DataHora.Date.Should().Be(DateTime.Now.Date, "a data deve ser inicializada com a data atual");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar erro quando valor for menor que o mínimo")]
    public void Validate_ValorMenorQueMinimo_DeveRetornarErro()
    {
        // Arrange
        var valorInvalido = 0.00m;
        var transacao = new Transacao(valorInvalido, "Descrição válida", TipoTransacao.Credito);

        // Act
        var resultado = transacao.Validate();

        // Assert
        resultado.IsValid.Should().BeFalse("uma transação com valor zero ou negativo não deve ser válida");
        resultado.Errors.Should().Contain(e => e.Message.Contains("O valor da operação deve ser maior que R$ 0"),
            "deve conter mensagem específica sobre o valor mínimo");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    [DisplayName("Deve retornar erro quando descrição for inválida")]
    public void Validate_DescricaoInvalida_DeveRetornarErro(string descricaoInvalida)
    {
        // Arrange
        var transacao = new Transacao(100.00m, descricaoInvalida, TipoTransacao.Credito);

        // Act
        var resultado = transacao.Validate();

        // Assert
        resultado.IsValid.Should()
            .BeFalse("uma transação com descrição vazia, nula ou apenas espaços não deve ser válida");
        resultado.Errors.Should().Contain(e => e.Message.Contains("descrição"),
            "deve conter mensagem específica sobre a descrição obrigatória");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar erro quando tipo for inválido")]
    public void Validate_TipoInvalido_DeveRetornarErro()
    {
        // Arrange
        var tipoInvalido = (TipoTransacao)999;
        var transacao = new Transacao(100.00m, "Descrição válida", tipoInvalido);

        // Act
        var resultado = transacao.Validate();

        // Assert
        resultado.IsValid.Should().BeFalse("uma transação com tipo inválido não deve ser válida");
        resultado.Errors.Should().Contain(e => e.Message.Contains("Tipo da operação inválido"),
            "deve conter mensagem específica sobre o tipo inválido");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar sucesso quando todos os parâmetros forem válidos")]
    public void Validate_ParametrosValidos_DeveRetornarSucesso()
    {
        // Arrange
        var transacao = new Transacao(100.00m, "Descrição válida", TipoTransacao.Credito);

        // Act
        var resultado = transacao.Validate();

        // Assert
        resultado.IsValid.Should().BeTrue("uma transação com todos os parâmetros válidos deve ser válida");
        resultado.Errors.Should().BeEmpty("não deve conter erros de validação");
    }
}