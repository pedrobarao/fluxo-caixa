using System.ComponentModel;
using FC.Cache;
using FC.Consolidado.Application.Queries;
using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace FC.Consolidado.Application.Test.Queries;

public class SaldoConsolidadoQueryTest
{
    private readonly string _cacheInstanceName = "test-instance";
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly SaldoConsolidadoQuery _query;
    private readonly RedisCacheOptions _redisCacheOptions;
    private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;

    public SaldoConsolidadoQueryTest()
    {
        _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
        _cacheServiceMock = new Mock<ICacheService>();

        _redisCacheOptions = new RedisCacheOptions { InstanceName = _cacheInstanceName };
        var optionsMock = new Mock<IOptions<RedisCacheOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_redisCacheOptions);

        _query = new SaldoConsolidadoQuery(
            _transacaoRepositoryMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar saldo do cache quando existir")]
    public async Task ObterPorData_SaldoExisteNoCache_DeveRetornarSaldoDoCache()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.Now);
        var saldoEsperado = new SaldoConsolidado(data);
        saldoEsperado.DefinirSaldoInicial(1000m);
        saldoEsperado.AdicionarTransacao(new Transacao(
            Guid.NewGuid(),
            500m,
            "Transação de teste",
            TipoTransacao.Credito,
            data.ToDateTime(TimeOnly.MinValue)));

        var chave = data.ToString("yyyy-MM-dd");

        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidado>(chave))
            .ReturnsAsync(saldoEsperado);

        // Act
        var resultado = await _query.ObterPorData(data);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().Be(data);
        resultado.SaldoInicial.Should().Be(1000m);
        resultado.TotalCreditos.Should().Be(500m);
        resultado.TotalDebitos.Should().Be(0m);
        resultado.SaldoFinal.Should().Be(1500m);

        _transacaoRepositoryMock.Verify(
            r => r.ObterTransacoesPorData(It.IsAny<DateOnly>()),
            Times.Never);

        _cacheServiceMock.Verify(
            c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), null),
            Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve buscar transações no repositório quando saldo não existir no cache")]
    public async Task ObterPorData_SaldoNaoExisteNoCache_DeveBuscarTransacoesNoRepositorio()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.Now);
        var transacoes = new List<Transacao>
        {
            new(Guid.NewGuid(), 100m, "Transação 1", TipoTransacao.Credito, data.ToDateTime(TimeOnly.MinValue)),
            new(Guid.NewGuid(), 50m, "Transação 2", TipoTransacao.Debito, data.ToDateTime(TimeOnly.MinValue))
        };

        var chave = data.ToString("yyyy-MM-dd");

        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidado>(chave))
            .ReturnsAsync((SaldoConsolidado?)null);

        _transacaoRepositoryMock
            .Setup(r => r.ObterTransacoesPorData(data))
            .ReturnsAsync(transacoes);

        _cacheServiceMock
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), null))
            .ReturnsAsync(true);

        // Act
        var resultado = await _query.ObterPorData(data);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().Be(data);
        resultado.SaldoInicial.Should().Be(0m);
        resultado.TotalCreditos.Should().Be(100m);
        resultado.TotalDebitos.Should().Be(50m);
        resultado.SaldoFinal.Should().Be(50m);

        _transacaoRepositoryMock.Verify(
            r => r.ObterTransacoesPorData(data),
            Times.Once);

        _cacheServiceMock.Verify(
            c => c.SetAsync(chave, It.IsAny<SaldoConsolidado>(), null),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve retornar saldo vazio quando não houver transações")]
    public async Task ObterPorData_SemTransacoes_DeveRetornarSaldoVazio()
    {
        // Arrange
        var data = DateOnly.FromDateTime(DateTime.Now);
        var transacoes = new List<Transacao>();

        var chave = data.ToString("yyyy-MM-dd");

        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidado>(chave))
            .ReturnsAsync((SaldoConsolidado?)null);

        _transacaoRepositoryMock
            .Setup(r => r.ObterTransacoesPorData(data))
            .ReturnsAsync(transacoes);

        _cacheServiceMock
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), null))
            .ReturnsAsync(true);

        // Act
        var resultado = await _query.ObterPorData(data);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().Be(data);
        resultado.SaldoInicial.Should().Be(0m);
        resultado.TotalCreditos.Should().Be(0m);
        resultado.TotalDebitos.Should().Be(0m);
        resultado.SaldoFinal.Should().Be(0m);

        _transacaoRepositoryMock.Verify(
            r => r.ObterTransacoesPorData(data),
            Times.Once);

        _cacheServiceMock.Verify(
            c => c.SetAsync(chave, It.IsAny<SaldoConsolidado>(), null),
            Times.Once);
    }
}