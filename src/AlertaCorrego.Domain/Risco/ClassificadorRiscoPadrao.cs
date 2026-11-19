using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Domain.Risco;

/// <summary>
/// Regra oficial do projeto (igual no Mobile, SOA e C#):
/// <list type="bullet">
/// <item>nível &gt;= nivelCriticoCm → CRITICO</item>
/// <item>nível &gt;= nivelAlertaCm → ALERTA</item>
/// <item>nível &gt;= 0,75 × nivelAlertaCm ou chuva &gt;= 30 mm/h → ATENCAO</item>
/// <item>caso contrário → NORMAL</item>
/// </list>
/// </summary>
public sealed class ClassificadorRiscoPadrao : IClassificadorRisco
{
    /// <summary>Fração do limite de alerta a partir da qual o risco é ATENCAO.</summary>
    public const double FatorAtencao = 0.75;

    /// <summary>Chuva (mm/h) a partir da qual o risco é, no mínimo, ATENCAO.</summary>
    public const double ChuvaAtencaoMmHora = 30.0;

    /// <summary>Instância compartilhada (a classe não tem estado).</summary>
    public static ClassificadorRiscoPadrao Instancia { get; } = new();

    /// <inheritdoc />
    public NivelRisco Classificar(double nivelAguaCm, double chuvaMmHora, double nivelAlertaCm, double nivelCriticoCm)
    {
        if (nivelAguaCm >= nivelCriticoCm)
        {
            return NivelRisco.Critico;
        }

        if (nivelAguaCm >= nivelAlertaCm)
        {
            return NivelRisco.Alerta;
        }

        if (nivelAguaCm >= FatorAtencao * nivelAlertaCm || chuvaMmHora >= ChuvaAtencaoMmHora)
        {
            return NivelRisco.Atencao;
        }

        return NivelRisco.Normal;
    }
}
