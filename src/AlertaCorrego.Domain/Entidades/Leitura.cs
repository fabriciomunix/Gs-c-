using AlertaCorrego.Domain.Comum;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Risco;

namespace AlertaCorrego.Domain.Entidades;

/// <summary>
/// Medição enviada pelo sensor: nível da água (cm) e chuva (mm/h).
/// </summary>
public sealed class Leitura : EntidadeBase
{
    public const double NivelMaximoCm = 10_000;
    public const double ChuvaMaximaMmHora = 500;

    private Leitura()
    {
    }

    public Leitura(int sensorId, double nivelAguaCm, double chuvaMmHora, DateTime registradaEm)
    {
        SensorId = sensorId;
        NivelAguaCm = Guarda.Faixa(nivelAguaCm, 0, NivelMaximoCm, nameof(NivelAguaCm));
        ChuvaMmHora = Guarda.Faixa(chuvaMmHora, 0, ChuvaMaximaMmHora, nameof(ChuvaMmHora));
        RegistradaEm = registradaEm;
        NivelRisco = NivelRisco.Normal;
    }

    public int SensorId { get; private set; }

    /// <summary>Sensor que gerou a leitura (navegação do EF Core).</summary>
    public Sensor Sensor { get; private set; } = null!;

    public double NivelAguaCm { get; private set; }

    public double ChuvaMmHora { get; private set; }

    public DateTime RegistradaEm { get; private set; }

    /// <summary>Risco calculado no momento do registro (persistido para histórico).</summary>
    public NivelRisco NivelRisco { get; private set; }

    /// <summary>Indica se a leitura exige a criação de um alerta.</summary>
    public bool ExigeAlerta => this.NivelRisco.ExigeAlerta();

    /// <summary>
    /// Classifica (e guarda) o risco desta leitura segundo os limites do córrego.
    /// </summary>
    public NivelRisco ClassificarRisco(Corrego corrego, IClassificadorRisco? classificador = null)
    {
        ArgumentNullException.ThrowIfNull(corrego);
        NivelRisco = corrego.AvaliarRisco(NivelAguaCm, ChuvaMmHora, classificador);
        return NivelRisco;
    }
}
