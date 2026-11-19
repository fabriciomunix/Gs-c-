using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Domain.Risco;

/// <summary>
/// Estratégia (Strategy) de classificação de risco. Permite trocar a regra
/// (ex.: uma regra calibrada por córrego) sem alterar as entidades.
/// </summary>
public interface IClassificadorRisco
{
    /// <summary>Classifica o risco a partir do nível da água, da chuva e dos limites do córrego.</summary>
    NivelRisco Classificar(double nivelAguaCm, double chuvaMmHora, double nivelAlertaCm, double nivelCriticoCm);
}
