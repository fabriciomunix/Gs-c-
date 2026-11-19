using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Risco;

namespace AlertaCorrego.Tests.Dominio;

public class ClassificadorRiscoPadraoTests
{
    private const double NivelAlerta = 120;
    private const double NivelCritico = 180;

    private readonly ClassificadorRiscoPadrao _classificador = new();

    [Theory]
    [InlineData(0.0, 0.0, NivelRisco.Normal)]
    [InlineData(60.0, 5.0, NivelRisco.Normal)]
    [InlineData(89.9, 29.9, NivelRisco.Normal)]          // logo abaixo de 75% e de 30 mm/h
    [InlineData(90.0, 0.0, NivelRisco.Atencao)]              // exatamente 75% de 120
    [InlineData(50.0, 30.0, NivelRisco.Atencao)]             // chuva exatamente 30 mm/h
    [InlineData(119.9, 0.0, NivelRisco.Atencao)]
    [InlineData(120.0, 0.0, NivelRisco.Alerta)]              // exatamente o limite de alerta
    [InlineData(179.9, 80.0, NivelRisco.Alerta)]           // chuva forte não eleva acima de ALERTA
    [InlineData(180.0, 0.0, NivelRisco.Critico)]             // exatamente o limite crítico
    [InlineData(250.0, 100.0, NivelRisco.Critico)]
    public void Classificar_DeveSeguirRegraDaFicha(double nivel, double chuva, NivelRisco esperado)
    {
        var risco = _classificador.Classificar(nivel, chuva, NivelAlerta, NivelCritico);

        Assert.Equal(esperado, risco);
    }

    [Theory]
    [InlineData(NivelRisco.Normal, false)]
    [InlineData(NivelRisco.Atencao, false)]
    [InlineData(NivelRisco.Alerta, true)]
    [InlineData(NivelRisco.Critico, true)]
    public void ExigeAlerta_SomenteParaAlertaECritico(NivelRisco nivel, bool esperado)
    {
        Assert.Equal(esperado, nivel.ExigeAlerta());
    }

    [Fact]
    public void Corrego_AvaliarRisco_UsaEstrategiaInformada()
    {
        var corrego = new Corrego("Córrego Teste", "Bairro Teste", -23.6, -46.7, NivelAlerta, NivelCritico);
        var estrategiaSempreCritico = new EstrategiaFixa(NivelRisco.Critico);

        Assert.Equal(NivelRisco.Normal, corrego.AvaliarRisco(10, 0));
        Assert.Equal(NivelRisco.Critico, corrego.AvaliarRisco(10, 0, estrategiaSempreCritico));
    }

    private sealed class EstrategiaFixa : IClassificadorRisco
    {
        private readonly NivelRisco _resultado;

        public EstrategiaFixa(NivelRisco resultado) => _resultado = resultado;

        public NivelRisco Classificar(double nivelAguaCm, double chuvaMmHora, double nivelAlertaCm, double nivelCriticoCm) =>
            _resultado;
    }
}
