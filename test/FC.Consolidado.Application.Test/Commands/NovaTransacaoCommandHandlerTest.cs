using System.ComponentModel;
using FC.Cache;
using FC.Consolidado.Application.Builders;
using FC.Consolidado.Application.Commands;
using FC.Consolidado.Domain.Entities;
using FC.Consolidado.Domain.Repositories;
using FC.Core.Data;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

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

        // Configuração das opções de cache
        _redisCacheOptions = new RedisCacheOptions { InstanceName = _cacheInstanceName };
        var optionsMock = new Mock<IOptions<RedisCacheOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_redisCacheOptions);

        // Configuração do UnitOfWork
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _transacaoRepositoryMock.Setup(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

        // Criação do handler a ser testado
        _handler = new NovaTransacaoCommandHandler(
            _transacaoRepositoryMock.Object,
            _cacheServiceMock.Object,
            optionsMock.Object);
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

        // Comando a ser processado
        var command = CriarComandoTransacao(transacaoId, 100.00m, "Descrição válida", TipoTransacao.Credito, dataHora);

        // Configurar saldos no cache
        var saldoAnterior = CriarSaldoAnteriorComCredito(dataAnterior, 500m, 100m);
        var saldoAtual = new SaldoConsolidado(dataAtual);

        // Configurar chaves de cache
        var cacheKeyBuilder = new SaldoConsolidadoCacheKeyBuilder(_redisCacheOptions.InstanceName!);
        var chaveAnterior = cacheKeyBuilder.BuildKey(dataAnterior);
        var chaveAtual = cacheKeyBuilder.BuildKey(dataAtual);

        // Configurar comportamento do cache
        ConfigurarCacheParaRetornarSaldos(chaveAnterior, saldoAnterior, chaveAtual, saldoAtual);
        ConfigurarCacheParaSalvarSaldo();

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Verificar resultado
        resultado.IsSuccess.Should().BeTrue("o processamento da transação deve ser bem-sucedido");
        resultado.Value.Should().NotBeNull("deve retornar um saldo consolidado");
        resultado.Value.SaldoInicial.Should().Be(saldoAnterior.SaldoFinal,
            "o saldo inicial deve ser igual ao saldo final do dia anterior");

        // Verificar se a transação foi adicionada ao repositório
        VerificarTransacaoAdicionadaAoRepositorio(command);

        // Verificar se o commit foi chamado
        _transacaoRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);

        // Verificar se o saldo foi atualizado no cache
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

        // Comando a ser processado
        var command = CriarComandoTransacao(transacaoId, 100.00m, "Descrição válida", TipoTransacao.Credito, dataHora);

        // Configurar chave de cache para o dia atual
        var cacheKeyBuilder = new SaldoConsolidadoCacheKeyBuilder(_redisCacheOptions.InstanceName!);
        var chaveAtual = cacheKeyBuilder.BuildKey(dataAtual);

        // Configurar cache para retornar null (saldo não existe)
        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidado>(It.IsAny<string>()))
            .ReturnsAsync((SaldoConsolidado?)null);

        // Configurar repositório para retornar lista vazia de transações
        _transacaoRepositoryMock
            .Setup(r => r.ObterTransacoesPorData(It.IsAny<DateOnly>()))
            .ReturnsAsync(new List<Transacao>());

        // Configurar cache para salvar saldo
        ConfigurarCacheParaSalvarSaldo();

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should()
            .BeTrue("o processamento da transação deve ser bem-sucedido mesmo sem saldo prévio");
        resultado.Value.Should().NotBeNull("deve retornar um novo saldo consolidado");

        // Verificar se buscou transações no repositório
        _transacaoRepositoryMock.Verify(r => r.ObterTransacoesPorData(It.IsAny<DateOnly>()), Times.AtLeastOnce);

        // Verificar se a transação foi adicionada ao repositório
        _transacaoRepositoryMock.Verify(r => r.Add(It.Is<Transacao>(t => t.Id == command.Id)), Times.Once);

        // Verificar se o saldo foi salvo no cache - usando a chave exata em vez de Contains
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

        // Comando a ser processado
        var command =
            CriarComandoTransacao(transacaoId, valorCredito, "Crédito teste", TipoTransacao.Credito, dataHora);

        // Configurar saldos no cache
        var saldoAnterior = CriarSaldoAnteriorComCredito(dataAnterior, saldoInicialAnterior, 100m);
        var saldoFinalAnterior = saldoAnterior.SaldoFinal;

        // Verificar que o saldo final anterior é 600m (500m inicial + 100m de crédito)
        saldoFinalAnterior.Should().Be(600m, "o saldo final anterior deve ser o saldo inicial mais o crédito anterior");

        // Criar saldo atual vazio (sem transações)
        var saldoAtual = new SaldoConsolidado(dataAtual);

        // Definir o saldo inicial do dia atual como o saldo final do dia anterior
        saldoAtual.DefinirSaldoInicial(saldoFinalAnterior);

        // Configurar chaves de cache
        var cacheKeyBuilder = new SaldoConsolidadoCacheKeyBuilder(_redisCacheOptions.InstanceName!);
        var chaveAnterior = cacheKeyBuilder.BuildKey(dataAnterior);
        var chaveAtual = cacheKeyBuilder.BuildKey(dataAtual);

        // Configurar comportamento do cache
        ConfigurarCacheParaRetornarSaldos(chaveAnterior, saldoAnterior, chaveAtual, saldoAtual);

        // Capturar o saldo atualizado que será salvo no cache
        SaldoConsolidado? saldoAtualizado = null;
        _cacheServiceMock
            .Setup(c => c.SetAsync(It.Is<string>(s => s == chaveAtual), It.IsAny<object>(), null))
            .Callback<string, object, TimeSpan?>((key, saldo, _) => saldoAtualizado = (SaldoConsolidado)saldo)
            .ReturnsAsync(true);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        saldoAtualizado.Should().NotBeNull("o saldo deve ser atualizado no cache");

        // Verificar saldo inicial
        saldoAtualizado!.SaldoInicial.Should().Be(saldoFinalAnterior,
            "o saldo inicial deve ser igual ao saldo final do dia anterior");

        // Verificar total de créditos
        saldoAtualizado.TotalCreditos.Should().Be(valorCredito,
            "o total de créditos deve incluir o valor da transação");

        // Verificar saldo final (saldo inicial + créditos - débitos)
        // Vamos imprimir os valores para depuração
        Console.WriteLine($"SaldoInicial: {saldoAtualizado.SaldoInicial}");
        Console.WriteLine($"TotalCreditos: {saldoAtualizado.TotalCreditos}");
        Console.WriteLine($"TotalDebitos: {saldoAtualizado.TotalDebitos}");
        Console.WriteLine($"SaldoFinal: {saldoAtualizado.SaldoFinal}");
        Console.WriteLine($"Esperado: {saldoFinalAnterior + valorCredito}");

        // Verificar a fórmula de cálculo do saldo final
        var saldoFinalCalculado =
            saldoAtualizado.SaldoInicial + saldoAtualizado.TotalCreditos - saldoAtualizado.TotalDebitos;
        Console.WriteLine($"SaldoFinalCalculado: {saldoFinalCalculado}");

        saldoAtualizado.SaldoFinal.Should().Be(saldoFinalAnterior + valorCredito,
            "o saldo final deve ser o saldo inicial mais o valor do crédito");
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

        // Comando a ser processado
        var command = CriarComandoTransacao(transacaoId, valorDebito, "Débito teste", TipoTransacao.Debito, dataHora);

        // Configurar saldos no cache
        var saldoAnterior = CriarSaldoAnteriorComCredito(dataAnterior, saldoInicialAnterior, 100m);
        var saldoFinalAnterior = saldoAnterior.SaldoFinal;

        // Verificar que o saldo final anterior é 600m (500m inicial + 100m de crédito)
        saldoFinalAnterior.Should().Be(600m, "o saldo final anterior deve ser o saldo inicial mais o crédito anterior");

        // Criar saldo atual vazio (sem transações)
        var saldoAtual = new SaldoConsolidado(dataAtual);

        // Definir o saldo inicial do dia atual como o saldo final do dia anterior
        saldoAtual.DefinirSaldoInicial(saldoFinalAnterior);

        // Configurar chaves de cache
        var cacheKeyBuilder = new SaldoConsolidadoCacheKeyBuilder(_redisCacheOptions.InstanceName!);
        var chaveAnterior = cacheKeyBuilder.BuildKey(dataAnterior);
        var chaveAtual = cacheKeyBuilder.BuildKey(dataAtual);

        // Configurar comportamento do cache
        ConfigurarCacheParaRetornarSaldos(chaveAnterior, saldoAnterior, chaveAtual, saldoAtual);

        // Capturar o saldo atualizado que será salvo no cache
        SaldoConsolidado? saldoAtualizado = null;
        _cacheServiceMock
            .Setup(c => c.SetAsync(It.Is<string>(s => s == chaveAtual), It.IsAny<object>(), null))
            .Callback<string, object, TimeSpan?>((key, saldo, _) => saldoAtualizado = (SaldoConsolidado)saldo)
            .ReturnsAsync(true);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        saldoAtualizado.Should().NotBeNull("o saldo deve ser atualizado no cache");

        // Verificar saldo inicial
        saldoAtualizado!.SaldoInicial.Should().Be(saldoFinalAnterior,
            "o saldo inicial deve ser igual ao saldo final do dia anterior");

        // Verificar total de débitos
        saldoAtualizado.TotalDebitos.Should().Be(valorDebito,
            "o total de débitos deve incluir o valor da transação");

        // Verificar saldo final (saldo inicial + créditos - débitos)
        saldoAtualizado.SaldoFinal.Should().Be(saldoFinalAnterior - valorDebito,
            "o saldo final deve ser o saldo inicial menos o valor do débito");
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

    private void ConfigurarCacheParaRetornarSaldos(string chaveAnterior, SaldoConsolidado saldoAnterior,
        string chaveAtual, SaldoConsolidado saldoAtual)
    {
        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidado>(chaveAnterior))
            .ReturnsAsync(saldoAnterior);

        _cacheServiceMock
            .Setup(c => c.GetAsync<SaldoConsolidado>(chaveAtual))
            .ReturnsAsync(saldoAtual);
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