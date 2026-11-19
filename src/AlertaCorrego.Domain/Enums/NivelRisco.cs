namespace AlertaCorrego.Domain.Enums;

/// <summary>
/// Nível de risco de transbordamento do córrego.
/// Serializado na API como NORMAL | ATENCAO | ALERTA | CRITICO.
/// </summary>
public enum NivelRisco
{
    /// <summary>Nível abaixo de 75% do limite de alerta e chuva fraca.</summary>
    Normal = 0,

    /// <summary>Nível a partir de 75% do limite de alerta ou chuva &gt;= 30 mm/h.</summary>
    Atencao = 1,

    /// <summary>Nível a partir do limite de alerta do córrego.</summary>
    Alerta = 2,

    /// <summary>Nível a partir do limite crítico do córrego.</summary>
    Critico = 3
}

/// <summary>Comportamentos auxiliares do enum <see cref="NivelRisco"/>.</summary>
public static class NivelRiscoExtensions
{
    /// <summary>Indica se o nível exige a criação de um alerta (ALERTA ou CRITICO).</summary>
    public static bool ExigeAlerta(this NivelRisco nivelRisco) =>
        nivelRisco is NivelRisco.Alerta or NivelRisco.Critico;
}
