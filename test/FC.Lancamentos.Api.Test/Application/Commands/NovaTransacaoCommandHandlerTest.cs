using System.ComponentModel;
using FC.Core.IntegrationEvents;
using FC.Lancamentos.Api.Application.Commands;
using FC.Lancamentos.Api.Domain.Entities;
using FluentAssertions;
using MassTransit;
using Moq;

namespace FC.Lancamentos.Api.Test.Application.Commands;

public class NovaTransacaoCommandHandlerTest
{
    private readonly Mock<IBus> _busMock;
    private readonly NovaTransacaoCommandHandler _handler;

    public NovaTransacaoCommandHandlerTest()
    {
        _busMock = new Mock<IBus>();
        _handler = new NovaTransacaoCommandHandler(_busMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar sucesso quando comando for válido")]
    public async Task Handle_ComandoValido_DeveRetornarSucesso()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 100.00m,
            Descricao = "Descrição válida",
            Tipo = TipoTransacao.Credito
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue("o comando com dados válidos deve ser processado com sucesso");
        resultado.Errors.Should().BeEmpty("não deve conter erros quando o comando é válido");

        _busMock.Verify(
            b => b.Publish(
                It.Is<TransacaoCriadaEvent>(e =>
                    e.Descricao == command.Descricao &&
                    e.Valor == command.Valor &&
                    e.Tipo == command.Tipo.ToString()),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar falha quando valor for inválido")]
    public async Task Handle_ValorInvalido_DeveRetornarFalha()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 0,
            Descricao = "Descrição válida",
            Tipo = TipoTransacao.Credito
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse("o comando com valor inválido deve falhar");
        resultado.Errors.Should().Contain(e => e.Message.Contains("valor"),
            "deve conter mensagem de erro relacionada ao valor");

        _busMock.Verify(
            b => b.Publish(It.IsAny<TransacaoCriadaEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar falha quando descrição for inválida")]
    public async Task Handle_DescricaoInvalida_DeveRetornarFalha()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 100.00m,
            Descricao = "",
            Tipo = TipoTransacao.Credito
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse("o comando com descrição inválida deve falhar");
        resultado.Errors.Should().Contain(e => e.Message.Contains("descrição"),
            "deve conter mensagem de erro relacionada à descrição");

        _busMock.Verify(
            b => b.Publish(It.IsAny<TransacaoCriadaEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar falha quando tipo for inválido")]
    public async Task Handle_TipoInvalido_DeveRetornarFalha()
    {
        // Arrange
        var command = new NovaTransacaoCommand
        {
            Valor = 100.00m,
            Descricao = "Descrição válida",
            Tipo = (TipoTransacao)999
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeFalse("o comando com tipo inválido deve falhar");
        resultado.Errors.Should().Contain(e => e.Message.Contains("Tipo"),
            "deve conter mensagem de erro relacionada ao tipo");

        _busMock.Verify(
            b => b.Publish(It.IsAny<TransacaoCriadaEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}