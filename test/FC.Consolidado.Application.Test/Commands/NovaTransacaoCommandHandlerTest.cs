using System.ComponentModel;
using FC.Cache;
using FC.Consolidado.Application.Commands;
using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Repositories;
using FC.Core.Data;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using FC.Consolidado.Application.DTOs;
using FC.Consolidado.Application.Mappings;

namespace FC.Consolidado.Application.Test.Commands;

public class NovaTransacaoCommandHandlerTest
{
    private readonly string _cacheInstanceName = "test-instance";
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly NovaTransacaoCommandHandler _handler;
    private readonly RedisCacheOptions _redisCacheOptions;
    private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;

    public NovaTransacaoCommandHandlerTest()
    {
        // Arrange - Configuração comum para todos os testes
        _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
        _cacheServiceMock = new Mock<ICacheService>();

        _redisCacheOptions = new RedisCacheOptions { InstanceName = _cacheInstanceName };
        var optionsMock = new Mock<IOptions<RedisCacheOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_redisCacheOptions);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _transacaoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        _handler = new NovaTransacaoCommandHandler(
            _transacaoRepositoryMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve processar transação e retornar saldo consolidado atualizado")]
    public async Task Handle_TransacaoValida_DeveProcessarERetornarSaldoAtualizado()
    {
        // Arrange
        var dataHora = DateTime.Now;
        var dataAtual = DateOnly.FromDateTime(dataHora);
        var dataAnterior = dataAtual.AddDays(-1);
        var transacaoId = Guid.NewGuid();

        var command = CriarComandoTransacao(transacaoId, 100.00m, "Descrição válida", TipoTransacao.Credito, dataHora);

        var saldoAnterior = CriarSaldoAnteriorComCredito(dataAnterior, 500m, 100m);
        var saldoAtual = new SaldoConsolidado(dataAtual);

        var chaveAtual = dataAtual.ToString("yyyy-MM-dd");

        ConfigurarCacheParaRetornarSaldos(dataAnterior, saldoAnterior, dataAtual, saldoAtual);
        ConfigurarCacheParaSalvarSaldo();

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.SaldoInicial.Should().Be(saldoAnterior.SaldoFinal);

        VerificarTransacaoAdicionadaAoRepositorio(command);

        _transacaoRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);

        VerificarSaldoAtualizadoNoCache(chaveAtual);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve criar novo saldo consolidado quando não existir no cache")]
    public async Task Handle_SaldoNaoExisteNoCache_DeveCriarNovoSaldo()
    {
        // Arrange
        var dataHora = DateTime.Now;
        var dataAtual = DateOnly.FromDateTime(dataHora);
        var transacaoId = Guid.NewGuid();

        var command = CriarComandoTransacao(transacaoId, 100.00m, "Descrição válida", TipoTransacao.Credito, dataHora);

        var chaveAtual = dataAtual.ToString("yyyy-MM-dd");

        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidadoDto>(It.IsAny<string>()))
            .ReturnsAsync((SaldoConsolidadoDto?)null);

        _transacaoRepositoryMock
            .Setup(r => r.ObterTransacoesPorData(It.IsAny<DateOnly>()))
            .ReturnsAsync(new List<Transacao>());

        ConfigurarCacheParaSalvarSaldo();

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();

        _transacaoRepositoryMock.Verify(r => r.ObterTransacoesPorData(It.IsAny<DateOnly>()), Times.AtLeastOnce);

        _transacaoRepositoryMock.Verify(r => r.Add(It.Is<Transacao>(t => t.Id == command.Id)), Times.Once);

        _cacheServiceMock.Verify(
            c => c.SetAsync(It.Is<string>(s => s == chaveAtual), It.IsAny<object>(), null),
            Times.AtLeastOnce);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve calcular saldo corretamente para transação de crédito")]
    public async Task Handle_TransacaoCredito_DeveCalcularSaldoCorretamente()
    {
        // Arrange
        var dataHora = DateTime.Now;
        var dataAtual = DateOnly.FromDateTime(dataHora);
        var dataAnterior = dataAtual.AddDays(-1);
        var transacaoId = Guid.NewGuid();
        var valorCredito = 150.00m;
        var saldoInicialAnterior = 500m;

        var command =
            CriarComandoTransacao(transacaoId, valorCredito, "Crédito teste", TipoTransacao.Credito, dataHora);

        var saldoAnterior = CriarSaldoAnteriorComCredito(dataAnterior, saldoInicialAnterior, 100m);
        var saldoFinalAnterior = saldoAnterior.SaldoFinal;

        saldoFinalAnterior.Should().Be(600m);

        var saldoAtual = new SaldoConsolidado(dataAtual);

        saldoAtual.DefinirSaldoInicial(saldoFinalAnterior);

        var chaveAtual = dataAtual.ToString("yyyy-MM-dd");

        ConfigurarCacheParaRetornarSaldos(dataAnterior, saldoAnterior, dataAtual, saldoAtual);

        SaldoConsolidadoDto? saldoAtualizado = null;
        _cacheServiceMock
            .Setup(c => c.SetAsync(It.Is<string>(s => s == chaveAtual), It.IsAny<object>(), null))
            .Callback<string, object, TimeSpan?>((key, saldo, _) => saldoAtualizado = (SaldoConsolidadoDto)saldo)
            .ReturnsAsync(true);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        saldoAtualizado.Should().NotBeNull();

        saldoAtualizado!.SaldoInicial.Should().Be(saldoFinalAnterior);

        saldoAtualizado.TotalCreditos.Should().Be(valorCredito);

        var saldoFinalCalculado =
            saldoAtualizado.SaldoInicial + saldoAtualizado.TotalCreditos - saldoAtualizado.TotalDebitos;

        saldoAtualizado.SaldoFinal.Should().Be(saldoFinalAnterior + valorCredito);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Deve calcular saldo corretamente para transação de débito")]
    public async Task Handle_TransacaoDebito_DeveCalcularSaldoCorretamente()
    {
        // Arrange
        var dataHora = DateTime.Now;
        var dataAtual = DateOnly.FromDateTime(dataHora);
        var dataAnterior = dataAtual.AddDays(-1);
        var transacaoId = Guid.NewGuid();
        var valorDebito = 150.00m;
        var saldoInicialAnterior = 500m;

        var command = CriarComandoTransacao(transacaoId, valorDebito, "Débito teste", TipoTransacao.Debito, dataHora);

        var saldoAnterior = CriarSaldoAnteriorComCredito(dataAnterior, saldoInicialAnterior, 100m);
        var saldoFinalAnterior = saldoAnterior.SaldoFinal;

        saldoFinalAnterior.Should().Be(600m);

        var saldoAtual = new SaldoConsolidado(dataAtual);

        saldoAtual.DefinirSaldoInicial(saldoFinalAnterior);

        var chaveAtual = dataAtual.ToString("yyyy-MM-dd");

        ConfigurarCacheParaRetornarSaldos(dataAnterior, saldoAnterior, dataAtual, saldoAtual);

        SaldoConsolidadoDto? saldoAtualizado = null;
        _cacheServiceMock
            .Setup(c => c.SetAsync(It.Is<string>(s => s == chaveAtual), It.IsAny<object>(), null))
            .Callback<string, object, TimeSpan?>((key, saldo, _) => saldoAtualizado = (SaldoConsolidadoDto)saldo)
            .ReturnsAsync(true);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        saldoAtualizado.Should().NotBeNull();

        saldoAtualizado!.SaldoInicial.Should().Be(saldoFinalAnterior);

        saldoAtualizado.TotalDebitos.Should().Be(valorDebito);

        saldoAtualizado.SaldoFinal.Should().Be(saldoFinalAnterior - valorDebito);
    }

    #region Métodos auxiliares

    private NovaTransacaoCommand CriarComandoTransacao(Guid id, decimal valor, string descricao, TipoTransacao tipo,
        DateTime dataHora)
    {
        return new NovaTransacaoCommand
        {
            Id = id,
            Valor = valor,
            Descricao = descricao,
            Tipo = tipo,
            DataHora = dataHora
        };
    }

    private SaldoConsolidado CriarSaldoAnteriorComCredito(DateOnly data, decimal saldoInicial, decimal valorCredito)
    {
        var saldo = new SaldoConsolidado(data);
        saldo.DefinirSaldoInicial(saldoInicial);
        saldo.AdicionarTransacao(new Transacao(
            Guid.NewGuid(),
            valorCredito,
            "Transação anterior",
            TipoTransacao.Credito,
            data.ToDateTime(TimeOnly.MinValue)));
        return saldo;
    }

    private void ConfigurarCacheParaRetornarSaldos(DateOnly dataAnterior, SaldoConsolidado saldoAnterior,
        DateOnly dataAtual, SaldoConsolidado saldoAtual)
    {
        var chaveAnterior = dataAnterior.ToString("yyyy-MM-dd");
        var chaveAtual = dataAtual.ToString("yyyy-MM-dd");

        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidadoDto>(chaveAnterior))
            .ReturnsAsync(saldoAnterior.ToDto());

        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidadoDto>(chaveAtual))
            .ReturnsAsync(saldoAtual.ToDto());
    }

    private void ConfigurarCacheParaSalvarSaldo()
    {
        _cacheServiceMock
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<object>(), null))
            .ReturnsAsync(true);
    }

    private void VerificarTransacaoAdicionadaAoRepositorio(NovaTransacaoCommand command)
    {
        _transacaoRepositoryMock.Verify(
            r => r.Add(It.Is<Transacao>(t =>
                t.Id == command.Id &&
                t.Valor == command.Valor &&
                t.Descricao == command.Descricao &&
                t.Tipo == command.Tipo)),
            Times.Once);
    }

    private void VerificarSaldoAtualizadoNoCache(string chave)
    {
        _cacheServiceMock.Verify(
            c => c.SetAsync(It.Is<string>(s => s == chave), It.IsAny<object>(), null),
            Times.AtLeastOnce);
    }

    #endregion
}