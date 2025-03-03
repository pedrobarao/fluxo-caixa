using System.ComponentModel;
using FC.Lancamentos.Api.Application.Commands;
using FC.Lancamentos.Api.Application.Commands.Validators;
using FC.Lancamentos.Api.Domain.Entities;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace FC.Lancamentos.Api.Test.Application.Commands.Validators;

public class NovaTransacaoValidatorTest
{
    private readonly NovaTransacaoValidator _validator;

    public NovaTransacaoValidatorTest()
    {
        _validator = new NovaTransacaoValidator();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve validar comando com valores válidos")]
    public void ValidarComando_ValoresValidos_DeveValidarComSucesso()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 100.00m,
            Descricao = "Transação de teste",
            Tipo = TipoTransacao.Credito
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve falhar validação quando Valor for zero")]
    public void ValidarComando_ValorZero_DeveFalhar()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 0,
            Descricao = "Transação de teste",
            Tipo = TipoTransacao.Credito
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Valor);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve falhar validação quando Valor for negativo")]
    public void ValidarComando_ValorNegativo_DeveFalhar()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = -100.00m,
            Descricao = "Transação de teste",
            Tipo = TipoTransacao.Credito
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Valor);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve falhar validação quando Descrição for vazia")]
    public void ValidarComando_DescricaoVazia_DeveFalhar()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 100.00m,
            Descricao = "",
            Tipo = TipoTransacao.Credito
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Descricao);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve falhar validação quando Descrição for nula")]
    public void ValidarComando_DescricaoNula_DeveFalhar()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 100.00m,
            Descricao = null,
            Tipo = TipoTransacao.Credito
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Descricao);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve falhar validação quando Descrição exceder tamanho máximo")]
    public void ValidarComando_DescricaoMuitoLonga_DeveFalhar()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 100.00m,
            Descricao = new string('A', 251), // 251 caracteres
            Tipo = TipoTransacao.Credito
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(c => c.Descricao);
    }
}