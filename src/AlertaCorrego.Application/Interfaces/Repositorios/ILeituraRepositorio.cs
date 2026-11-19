using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Application.Interfaces.Repositorios;

/// <summary>Agregado das leituras de um nível de risco (resultado de GroupBy).</summary>
public sealed record AgregadoLeituras(
    NivelRisco NivelRisco,
    int Quantidade,
    double SomaNivelAguaCm,
    double MaximoNivelAguaCm,
    double MinimoNivelAguaCm,
    double MaximaChuvaMmHora);

public interface ILeituraRepositorio : IRepositorio<Leitura>
{
    /// <summary>Lista as leituras mais recentes (opcionalmente de um córrego), já projetadas.</summary>
    Task<IReadOnlyList<LeituraResponse>> ListarAsync(int? corregoId, int limite, CancellationToken cancellationToken = default);

    /// <summary>Última leitura de qualquer sensor do córrego.</summary>
    Task<LeituraResponse?> ObterUltimaPorCorregoAsync(int corregoId, CancellationToken cancellationToken = default);

    /// <summary>Agrupa por nível de risco as leituras do córrego no intervalo informado.</summary>
    Task<IReadOnlyList<AgregadoLeituras>> ObterAgregadosPorRiscoAsync(
        int corregoId,
        DateTime de,
        DateTime ate,
        CancellationToken cancellationToken = default);
}
