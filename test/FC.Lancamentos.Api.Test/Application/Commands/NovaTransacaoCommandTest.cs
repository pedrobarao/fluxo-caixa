using System.ComponentModel;
using FC.Lancamentos.Api.Application.Commands;
using FC.Lancamentos.Api.Domain.Entities;
using FluentAssertions;

namespace FC.Lancamentos.Api.Test.Application.Commands;

public class NovaTransacaoCommandTest
{
    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve criar comando com propriedades corretamente inicializadas")]
    public void NovaTransacaoCommand_CriarComando_DeveInicializarPropriedadesCorretamente()
    {
        // Arrange
        var valor = 100.00m;
        var descricao = "Pagamento de conta";
        var tipo = TipoTransacao.Debito;

        // Act
        var command = new NovaTransacaoCommand
        {
            Valor = valor,
            Descricao = descricao,
            Tipo = tipo
        };

        // Assert
        command.Valor.Should().Be(valor, "o valor deve ser inicializado com o valor informado");
        command.Descricao.Should().Be(descricao, "a descrição deve ser inicializada com o texto informado");
        command.Tipo.Should().Be(tipo, "o tipo deve ser inicializado com o valor informado");
    }
}