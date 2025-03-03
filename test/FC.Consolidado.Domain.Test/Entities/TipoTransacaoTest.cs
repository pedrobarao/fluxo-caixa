using System.ComponentModel;
using FC.Consolidado.Domain.Entities;
using FluentAssertions;

namespace FC.Consolidado.Domain.Test.Entities;

public class TipoTransacaoTest
{
    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve ter valores corretos para o enum TipoTransacao")]
    public void TipoTransacao_Valores_DevemSerCorretos()
    {
        ((int)TipoTransacao.Debito).Should().Be(0);
        ((int)TipoTransacao.Credito).Should().Be(1);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve converter corretamente entre string e enum")]
    public void TipoTransacao_Conversao_DeveConverterCorretamente()
    {
        var debitoString = TipoTransacao.Debito.ToString();
        var creditoString = TipoTransacao.Credito.ToString();

        var debitoEnum = Enum.Parse<TipoTransacao>(debitoString);
        var creditoEnum = Enum.Parse<TipoTransacao>(creditoString);

        debitoString.Should().Be("Debito");
        creditoString.Should().Be("Credito");

        debitoEnum.Should().Be(TipoTransacao.Debito);
        creditoEnum.Should().Be(TipoTransacao.Credito);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve lançar exceção ao tentar converter valor inválido")]
    public void TipoTransacao_ConversaoInvalida_DeveLancarExcecao()
    {
        var valorInvalido = "Invalido";

        Action act = () => Enum.Parse<TipoTransacao>(valorInvalido);

        act.Should().Throw<ArgumentException>();
    }
}