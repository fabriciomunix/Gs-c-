using AlertaCorrego.Domain.Comum;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;
using AlertaCorrego.Domain.Risco;

namespace AlertaCorrego.Domain.Entidades;

/// <summary>
/// Córrego monitorado. Guarda os limites (cm) que definem os níveis de risco.
/// </summary>
public sealed class Corrego : EntidadeBase
{
    public const int TamanhoMaximoNome = 120;
    public const int TamanhoMaximoBairro = 80;
    public const double NivelMaximoCm = 10_000;

    // Construtor exigido pelo EF Core.
    private Corrego()
    {
    }

    public Corrego(
        string nome,
        string bairro,
        double latitude,
        double longitude,
        double nivelAlertaCm,
        double nivelCriticoCm)
    {
        Atualizar(nome, bairro, latitude, longitude, nivelAlertaCm, nivelCriticoCm);
    }

    public string Nome { get; private set; } = string.Empty;

    public string Bairro { get; private set; } = string.Empty;

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    /// <summary>Nível (cm) a partir do qual o risco é ALERTA.</summary>
    public double NivelAlertaCm { get; private set; }

    /// <summary>Nível (cm) a partir do qual o risco é CRITICO.</summary>
    public double NivelCriticoCm { get; private set; }

    /// <summary>Atualiza todos os dados do córrego validando as invariantes.</summary>
    public void Atualizar(
        string nome,
        string bairro,
        double latitude,
        double longitude,
        double nivelAlertaCm,
        double nivelCriticoCm)
    {
        Nome = Guarda.TextoObrigatorio(nome, nameof(Nome), TamanhoMaximoNome, tamanhoMinimo: 3);
        Bairro = Guarda.TextoObrigatorio(bairro, nameof(Bairro), TamanhoMaximoBairro, tamanhoMinimo: 2);
        DefinirLocalizacao(latitude, longitude);
        DefinirLimites(nivelAlertaCm, nivelCriticoCm);
    }

    /// <summary>Define a coordenada geográfica do ponto monitorado.</summary>
    public void DefinirLocalizacao(double latitude, double longitude)
    {
        Latitude = Guarda.Faixa(latitude, -90, 90, nameof(Latitude));
        Longitude = Guarda.Faixa(longitude, -180, 180, nameof(Longitude));
    }

    /// <summary>
    /// Define os limites de risco. Invariante: 0 &lt; nivelAlertaCm &lt; nivelCriticoCm.
    /// </summary>
    public void DefinirLimites(double nivelAlertaCm, double nivelCriticoCm)
    {
        Guarda.Faixa(nivelAlertaCm, 1, NivelMaximoCm, nameof(NivelAlertaCm));
        Guarda.Faixa(nivelCriticoCm, 1, NivelMaximoCm, nameof(NivelCriticoCm));

        if (nivelCriticoCm <= nivelAlertaCm)
        {
            throw new ValidacaoDominioException(
                nameof(NivelCriticoCm),
                "O nível crítico deve ser maior que o nível de alerta.");
        }

        NivelAlertaCm = nivelAlertaCm;
        NivelCriticoCm = nivelCriticoCm;
    }

    /// <summary>
    /// Avalia o risco do córrego para um nível de água e uma intensidade de chuva,
    /// usando a estratégia informada (ou a regra padrão do projeto).
    /// </summary>
    public NivelRisco AvaliarRisco(double nivelAguaCm, double chuvaMmHora, IClassificadorRisco? classificador = null) =>
        (classificador ?? ClassificadorRiscoPadrao.Instancia)
            .Classificar(nivelAguaCm, chuvaMmHora, NivelAlertaCm, NivelCriticoCm);
}
