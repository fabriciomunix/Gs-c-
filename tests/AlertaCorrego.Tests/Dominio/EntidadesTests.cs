using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;
using AlertaCorrego.Domain.Notificacoes;

namespace AlertaCorrego.Tests.Dominio;

public class EntidadesTests
{
    private static Corrego NovoCorrego() =>
        new("Córrego Ponte Baixa", "M'Boi Mirim", -23.69, -46.77, 120, 180);

    [Fact]
    public void Corrego_NivelCriticoMenorOuIgualAoAlerta_DeveLancarValidacao()
    {
        var ex = Assert.Throws<ValidacaoDominioException>(
            () => new Corrego("Córrego X", "Bairro", -23.6, -46.7, 150, 150));

        Assert.Equal(nameof(Corrego.NivelCriticoCm), ex.Campo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    public void Corrego_NomeInvalido_DeveLancarValidacao(string nome)
    {
        Assert.Throws<ValidacaoDominioException>(
            () => new Corrego(nome, "Bairro", -23.6, -46.7, 120, 180));
    }

    [Fact]
    public void Corrego_LatitudeForaDaFaixa_DeveLancarValidacao()
    {
        Assert.Throws<ValidacaoDominioException>(
            () => new Corrego("Córrego X", "Bairro", -95, -46.7, 120, 180));
    }

    [Fact]
    public void Leitura_NivelNegativo_DeveLancarValidacao()
    {
        Assert.Throws<ValidacaoDominioException>(() => new Leitura(1, -1, 0, DateTime.UtcNow));
    }

    [Fact]
    public void Leitura_ClassificarRisco_GuardaRiscoEIndicaAlerta()
    {
        var leitura = new Leitura(1, 185, 40, DateTime.UtcNow);

        var risco = leitura.ClassificarRisco(NovoCorrego());

        Assert.Equal(NivelRisco.Critico, risco);
        Assert.Equal(NivelRisco.Critico, leitura.NivelRisco);
        Assert.True(leitura.ExigeAlerta);
    }

    [Fact]
    public void Sensor_Inativo_NaoPodeRegistrarLeitura()
    {
        var sensor = new Sensor("esp32-pb-09", 1, DateTime.UtcNow, ativo: false);

        Assert.Equal("ESP32-PB-09", sensor.Codigo);
        Assert.Throws<RegraNegocioException>(() => sensor.RegistrarLeitura(50, 0, DateTime.UtcNow));
    }

    [Fact]
    public void Sensor_Ativo_RegistraLeituraComValores()
    {
        var sensor = new Sensor("ESP32-PB-01", 1, DateTime.UtcNow);
        var agora = DateTime.UtcNow;

        var leitura = sensor.RegistrarLeitura(100, 12.5, agora);

        Assert.Equal(100, leitura.NivelAguaCm);
        Assert.Equal(12.5, leitura.ChuvaMmHora);
        Assert.Equal(agora, leitura.RegistradaEm);
    }

    [Theory]
    [InlineData(TipoNecessidade.Idoso)]
    [InlineData(TipoNecessidade.Cadeirante)]
    [InlineData(TipoNecessidade.Acamado)]
    [InlineData(TipoNecessidade.Outra)]
    public void Morador_ComNecessidade_EhPrioritarioAutomaticamente(TipoNecessidade tipo)
    {
        var morador = new Morador("Pessoa Fictícia", "11900000000", "Rua Fictícia, 1", 1, prioritario: false, tipo);

        Assert.True(morador.Prioritario);
    }

    [Fact]
    public void Morador_TelefoneEhNormalizadoParaDigitos()
    {
        var morador = new Morador("Pessoa Fictícia", "(11) 90000-0001", "Rua Fictícia, 1", 1, false, TipoNecessidade.Nenhuma);

        Assert.Equal("11900000001", morador.Telefone);
        Assert.False(morador.Prioritario);
    }

    [Fact]
    public void Morador_TelefoneInvalido_DeveLancarValidacao()
    {
        Assert.Throws<ValidacaoDominioException>(
            () => new Morador("Pessoa Fictícia", "1234", "Rua Fictícia, 1", 1, false, TipoNecessidade.Nenhuma));
    }

    [Fact]
    public void Alerta_NaoPodeSerCriadoParaRiscoNormal()
    {
        Assert.Throws<ValidacaoDominioException>(() => new Alerta(1, NivelRisco.Atencao, "Mensagem", 90));
    }

    [Fact]
    public void Alerta_Encerrar_DesativaEImpedeNovoEncerramento()
    {
        var corrego = NovoCorrego();
        var leitura = new Leitura(1, 130, 10, DateTime.UtcNow);
        leitura.ClassificarRisco(corrego);

        var alerta = Alerta.Gerar(corrego, leitura);
        Assert.True(alerta.Ativo);
        Assert.Equal(NivelRisco.Alerta, alerta.NivelRisco);

        alerta.Encerrar();

        Assert.False(alerta.Ativo);
        Assert.NotNull(alerta.EncerradoEm);
        Assert.Throws<RegraNegocioException>(() => alerta.Encerrar());
    }

    [Fact]
    public void Notificacao_Criar_EhPolimorficaEPriorizaQuemPrecisaDeAjuda()
    {
        var corrego = NovoCorrego();
        var leitura = new Leitura(1, 200, 50, DateTime.UtcNow);
        leitura.ClassificarRisco(corrego);
        var alerta = Alerta.Gerar(corrego, leitura);

        var acamado = new Morador("Pessoa A (fictícia)", "11900000001", "Rua Fictícia, 1", 1, true, TipoNecessidade.Acamado);
        var semNecessidade = new Morador("Pessoa B (fictícia)", "11900000002", "Rua Fictícia, 2", 1, false, TipoNecessidade.Nenhuma);

        var prioritaria = Notificacao.Criar(acamado, alerta);
        var padrao = Notificacao.Criar(semNecessidade, alerta);

        Assert.IsType<NotificacaoPrioritaria>(prioritaria);
        Assert.IsType<NotificacaoPadrao>(padrao);
        Assert.True(prioritaria.Prioridade > padrao.Prioridade);
        Assert.StartsWith("[PRIORIDADE]", prioritaria.MontarMensagem());
        Assert.DoesNotContain("Pessoa A", prioritaria.MontarMensagem()); // sem dados pessoais
    }
}
