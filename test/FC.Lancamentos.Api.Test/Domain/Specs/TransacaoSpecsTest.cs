using System.ComponentModel;
using FC.Lancamentos.Api.Domain.Entities;
using FC.Lancamentos.Api.Domain.Specs;
using FluentAssertions;

namespace FC.Lancamentos.Api.Test.Domain.Specs;

public class TransacaoSpecsTest
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(-1)]
    [DisplayName("Deve retornar falso quando valor for menor ou igual a zero")]
    public void TransacaoValorMinimoSpec_ValorInvalido_DeveRetornarFalso(decimal valorInvalido)
    {
        // Arrange
        var transacao = new Transacao(valorInvalido, "Descrição válida", TipoTransacao.Credito);
        var spec = new TransacaoValorMinimoSpec();

        // Act
        var resultado = spec.IsSatisfiedBy(transacao);

        // Assert
        resultado.Should()
            .BeFalse("a especificação de valor mínimo não deve ser satisfeita com valores zero ou negativos");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar verdadeiro quando valor for maior que zero")]
    public void TransacaoValorMinimoSpec_ValorValido_DeveRetornarVerdadeiro()
    {
        // Arrange
        var transacao = new Transacao(100.00m, "Descrição válida", TipoTransacao.Credito);
        var spec = new TransacaoValorMinimoSpec();

        // Act
        var resultado = spec.IsSatisfiedBy(transacao);

        // Assert
        resultado.Should().BeTrue("a especificação de valor mínimo deve ser satisfeita com valores maiores que zero");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    [DisplayName("Deve retornar falso quando descrição for vazia, nula ou apenas espaços")]
    public void TransacaoDescricaoObrigatoriaSpec_DescricaoInvalida_DeveRetornarFalso(string descricaoInvalida)
    {
        // Arrange
        var transacao = new Transacao(100.00m, descricaoInvalida, TipoTransacao.Credito);
        var spec = new TransacaoDescricaoObrigatoriaSpec();

        // Act
        var resultado = spec.IsSatisfiedBy(transacao);

        // Assert
        resultado.Should()
            .BeFalse(
                "a especificação de descrição obrigatória não deve ser satisfeita com descrição vazia, nula ou apenas espaços");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar verdadeiro quando descrição for preenchida")]
    public void TransacaoDescricaoObrigatoriaSpec_DescricaoValida_DeveRetornarVerdadeiro()
    {
        // Arrange
        var transacao = new Transacao(100.00m, "Descrição válida", TipoTransacao.Credito);
        var spec = new TransacaoDescricaoObrigatoriaSpec();

        // Act
        var resultado = spec.IsSatisfiedBy(transacao);

        // Assert
        resultado.Should()
            .BeTrue("a especificação de descrição obrigatória deve ser satisfeita com descrição preenchida");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar verdadeiro quando tipo for válido")]
    public void TransacaoTipoValidoSpec_TipoValido_DeveRetornarVerdadeiro()
    {
        // Arrange
        var transacao = new Transacao(100.00m, "Descrição válida", TipoTransacao.Credito);
        var spec = new TransacaoTipoValidoSpec();

        // Act
        var resultado = spec.IsSatisfiedBy(transacao);

        // Assert
        resultado.Should().BeTrue("a especificação de tipo válido deve ser satisfeita com um tipo de transação válido");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar falso quando tipo for inválido")]
    public void TransacaoTipoValidoSpec_TipoInvalido_DeveRetornarFalso()
    {
        // Arrange
        var tipoInvalido = (TipoTransacao)999;
        var transacao = new Transacao(100.00m, "Descrição válida", tipoInvalido);
        var spec = new TransacaoTipoValidoSpec();

        // Act
        var resultado = spec.IsSatisfiedBy(transacao);

        // Assert
        resultado.Should()
            .BeFalse("a especificação de tipo válido não deve ser satisfeita com um tipo de transação inválido");
    }
}