using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Servicos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;
using AlertaCorrego.Domain.Risco;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AlertaCorrego.Tests.Servicos;

public class LeituraServiceTests
{
    private readonly Mock<ISensorRepositorio> _sensores = new();
    private readonly Mock<ICorregoRepositorio> _corregos = new();
    private readonly Mock<ILeituraRepositorio> _leituras = new();
    private readonly Mock<IAlertaRepositorio> _alertas = new();
    private readonly Mock<IMoradorRepositorio> _moradores = new();
    private readonly Mock<IUnidadeDeTrabalho> _unidadeDeTrabalho = new();

    private readonly LeituraService _servico;

    public LeituraServiceTests()
    {
        var corrego = new Corrego("Córrego Ponte Baixa", "M'Boi Mirim", -23.69, -46.77, 120, 180);
        var sensor = new Sensor("ESP32-PB-01", 1, DateTime.UtcNow.AddDays(-10));

        _sensores
            .Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sensor);

        _corregos
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(corrego);

        _unidadeDeTrabalho
            .Setup(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        IReadOnlyList<Morador> moradores = new List<Morador>
        {
            new("Pessoa Idosa (fictícia)", "11900000001", "Rua Fictícia, 1", 1, true, TipoNecessidade.Idoso),
            new("Pessoa Sem Necessidade (fictícia)", "11900000002", "Rua Fictícia, 2", 1, false, TipoNecessidade.Nenhuma),
            new("Pessoa Acamada (fictícia)", "11900000003", "Rua Fictícia, 3", 1, true, TipoNecessidade.Acamado)
        };

        _moradores
            .Setup(r => r.ListarAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(moradores);

        _servico = new LeituraService(
            _sensores.Object,
            _corregos.Object,
            _leituras.Object,
            _alertas.Object,
            _moradores.Object,
            _unidadeDeTrabalho.Object,
            new ClassificadorRiscoPadrao(),
            TimeProvider.System,
            NullLogger<LeituraService>.Instance);
    }

    [Fact]
    public async Task RegistrarAsync_LeituraCritica_CriaAlertaERetornaPrioritariosOrdenados()
    {
        var resposta = await _servico.RegistrarAsync(new LeituraRequest(1, 185, 48));

        Assert.Equal(NivelRisco.Critico, resposta.NivelRisco);
        Assert.NotNull(resposta.Alerta);
        Assert.Equal(NivelRisco.Critico, resposta.Alerta!.NivelRisco);

        // Apenas os 2 prioritários, do mais urgente (acamado) ao menos urgente (idoso).
        Assert.Equal(2, resposta.MoradoresPrioritarios.Count);
        Assert.Equal(TipoNecessidade.Acamado, resposta.MoradoresPrioritarios[0].TipoNecessidade);
        Assert.Equal(TipoNecessidade.Idoso, resposta.MoradoresPrioritarios[1].TipoNecessidade);

        // Todos os moradores recebem notificação; as prioritárias vêm primeiro.
        Assert.Equal(3, resposta.Notificacoes.Count);
        Assert.Equal("PRIORITARIA", resposta.Notificacoes[0].Tipo);
        Assert.Equal("PADRAO", resposta.Notificacoes[^1].Tipo);

        _leituras.Verify(
            r => r.AdicionarAsync(It.Is<Leitura>(l => l.NivelRisco == NivelRisco.Critico), It.IsAny<CancellationToken>()),
            Times.Once);
        _alertas.Verify(
            r => r.AdicionarAsync(It.Is<Alerta>(a => a.NivelRisco == NivelRisco.Critico && a.Ativo), It.IsAny<CancellationToken>()),
            Times.Once);
        _unidadeDeTrabalho.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(50.0, 0.0, NivelRisco.Normal)]
    [InlineData(95.0, 10.0, NivelRisco.Atencao)]
    public async Task RegistrarAsync_LeituraSemRiscoDeAlerta_NaoCriaAlerta(double nivel, double chuva, NivelRisco esperado)
    {
        var resposta = await _servico.RegistrarAsync(new LeituraRequest(1, nivel, chuva));

        Assert.Equal(esperado, resposta.NivelRisco);
        Assert.Null(resposta.Alerta);
        Assert.Empty(resposta.MoradoresPrioritarios);
        Assert.Empty(resposta.Notificacoes);

        _alertas.Verify(r => r.AdicionarAsync(It.IsAny<Alerta>(), It.IsAny<CancellationToken>()), Times.Never);
        _moradores.Verify(r => r.ListarAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Never);
        _leituras.Verify(r => r.AdicionarAsync(It.IsAny<Leitura>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_SensorInexistente_LancaEntidadeNaoEncontrada()
    {
        _sensores
            .Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sensor?)null);

        await Assert.ThrowsAsync<EntidadeNaoEncontradaException>(
            () => _servico.RegistrarAsync(new LeituraRequest(99, 100, 10)));

        _unidadeDeTrabalho.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(501)]
    public async Task ListarAsync_LimiteInvalido_LancaValidacao(int limite)
    {
        await Assert.ThrowsAsync<ValidacaoDominioException>(
            () => _servico.ListarAsync(null, limite));
    }
}
