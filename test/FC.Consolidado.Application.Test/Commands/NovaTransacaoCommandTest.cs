using System.ComponentModel;
using FC.Consolidado.Application.Commands;
using FC.Consolidado.Domain.Entities;
using FluentAssertions;

namespace FC.Consolidado.Application.Test.Commands;

public class NovaTransacaoCommandTest
{
    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve criar comando com propriedades corretamente inicializadas")]
    public void NovaTransacaoCommand_CriarComando_DeveInicializarPropriedadesCorretamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var valor = 100.00m;
        var descricao = "Pagamento de conta";
        var tipo = TipoTransacao.Debito;
        var dataHora = DateTime.Now;

        // Act
        var command = new NovaTransacaoCommand
        {
            Id = id,
            Valor = valor,
            Descricao = descricao,
            Tipo = tipo,
            DataHora = dataHora
        };

        // Assert
        command.Id.Should().Be(id, "o ID deve ser inicializado com o valor informado");
        command.Valor.Should().Be(valor, "o valor deve ser inicializado com o valor informado");
        command.Descricao.Should().Be(descricao, "a descrição deve ser inicializada com o texto informado");
        command.Tipo.Should().Be(tipo, "o tipo deve ser inicializado com o valor informado");
        command.DataHora.Should().Be(dataHora, "a data/hora deve ser inicializada com o valor informado");
    }
}