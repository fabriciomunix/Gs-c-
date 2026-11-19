using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Servicos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;
using Moq;

namespace AlertaCorrego.Tests.Servicos;

public class CorregoServiceTests
{
    private readonly Mock<ICorregoRepositorio> _corregos = new();
    private readonly Mock<ILeituraRepositorio> _leituras = new();
    private readonly Mock<IAlertaRepositorio> _alertas = new();
    private readonly Mock<IUnidadeDeTrabalho> _unidadeDeTrabalho = new();
    private readonly CorregoService _servico;

    public CorregoServiceTests()
    {
        _servico = new CorregoService(
            _corregos.Object,
            _leituras.Object,
            _alertas.Object,
            _unidadeDeTrabalho.Object,
            TimeProvider.System);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_AdicionaESalva()
    {
        var request = new CorregoRequest("Córrego Novo", "Capão Redondo", -23.66, -46.77, 100, 160);

        var resposta = await _servico.CriarAsync(request);

        Assert.Equal("Córrego Novo", resposta.Nome);
        _corregos.Verify(r => r.AdicionarAsync(It.IsAny<Corrego>(), It.IsAny<CancellationToken>()), Times.Once);
        _unidadeDeTrabalho.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_LimitesInvalidos_LancaValidacaoENaoSalva()
    {
        var request = new CorregoRequest("Córrego Novo", "Capão Redondo", -23.66, -46.77, 160, 100);

        await Assert.ThrowsAsync<ValidacaoDominioException>(() => _servico.CriarAsync(request));
        _unidadeDeTrabalho.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ObterStatusAsync_SemLeituras_RetornaNormal()
    {
        var corrego = new Corrego("Córrego Teste", "Bairro", -23.6, -46.7, 120, 180);
        _corregos.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(corrego);
        _leituras.Setup(r => r.ObterUltimaPorCorregoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((LeituraResponse?)null);
        _alertas.Setup(r => r.ContarAtivosAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var status = await _servico.ObterStatusAsync(1);

        Assert.Equal(NivelRisco.Normal, status.NivelRisco);
        Assert.Null(status.UltimaLeitura);
    }

    [Fact]
    public async Task ObterEstatisticasAsync_CalculaMediaEMaximoAPartirDosGrupos()
    {
        var corrego = new Corrego("Córrego Teste", "Bairro", -23.6, -46.7, 120, 180);
        _corregos.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(corrego);

        IReadOnlyList<AgregadoLeituras> grupos = new List<AgregadoLeituras>
        {
            new(NivelRisco.Normal, 2, 100, 60, 40, 5),      // 40 + 60
            new(NivelRisco.Alerta, 1, 130, 130, 130, 45)
        };
        _leituras
            .Setup(r => r.ObterAgregadosPorRiscoAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(grupos);

        var estatisticas = await _servico.ObterEstatisticasAsync(1, 24);

        Assert.Equal(3, estatisticas.TotalLeituras);
        Assert.Equal(76.67, estatisticas.MediaNivelAguaCm);
        Assert.Equal(130, estatisticas.MaximoNivelAguaCm);
        Assert.Equal(40, estatisticas.MinimoNivelAguaCm);
        Assert.Equal(45, estatisticas.MaximaChuvaMmHora);
        Assert.Equal(2, estatisticas.PorNivelRisco.Count);
    }

    [Fact]
    public async Task ObterPorIdAsync_Inexistente_LancaEntidadeNaoEncontrada()
    {
        _corregos
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Corrego?)null);

        await Assert.ThrowsAsync<EntidadeNaoEncontradaException>(() => _servico.ObterPorIdAsync(7));
    }
}
