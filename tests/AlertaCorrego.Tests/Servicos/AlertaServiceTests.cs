using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Servicos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;
using Moq;

namespace AlertaCorrego.Tests.Servicos;

public class AlertaServiceTests
{
    private readonly Mock<IAlertaRepositorio> _alertas = new();
    private readonly Mock<IUnidadeDeTrabalho> _unidadeDeTrabalho = new();
    private readonly AlertaService _servico;

    public AlertaServiceTests()
    {
        _servico = new AlertaService(_alertas.Object, _unidadeDeTrabalho.Object, TimeProvider.System);
    }

    [Fact]
    public async Task EncerrarAsync_AlertaAtivo_EncerraESalva()
    {
        var alerta = new Alerta(1, NivelRisco.Alerta, "ALERTA: teste", 125);
        _alertas
            .Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerta);

        var resposta = await _servico.EncerrarAsync(1);

        Assert.False(resposta.Ativo);
        Assert.NotNull(resposta.EncerradoEm);
        _unidadeDeTrabalho.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EncerrarAsync_AlertaInexistente_LancaEntidadeNaoEncontrada()
    {
        _alertas
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Alerta?)null);

        await Assert.ThrowsAsync<EntidadeNaoEncontradaException>(() => _servico.EncerrarAsync(42));
    }

    [Fact]
    public async Task EncerrarAsync_AlertaJaEncerrado_LancaRegraNegocio()
    {
        var alerta = new Alerta(1, NivelRisco.Critico, "RISCO CRÍTICO: teste", 190);
        alerta.Encerrar();
        _alertas
            .Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerta);

        await Assert.ThrowsAsync<RegraNegocioException>(() => _servico.EncerrarAsync(1));
        _unidadeDeTrabalho.Verify(u => u.SalvarAlteracoesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
