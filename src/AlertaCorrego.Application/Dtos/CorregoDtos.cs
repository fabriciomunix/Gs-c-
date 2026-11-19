using System.ComponentModel.DataAnnotations;
using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Application.Dtos;

/// <summary>Dados para cadastrar ou atualizar um córrego.</summary>
/// <param name="Nome">Nome do córrego (ex.: Córrego Ponte Baixa).</param>
/// <param name="Bairro">Bairro/região (ex.: M'Boi Mirim).</param>
/// <param name="Latitude">Latitude do ponto monitorado.</param>
/// <param name="Longitude">Longitude do ponto monitorado.</param>
/// <param name="NivelAlertaCm">Nível (cm) a partir do qual o risco é ALERTA.</param>
/// <param name="NivelCriticoCm">Nível (cm) a partir do qual o risco é CRITICO (maior que o de alerta).</param>
public sealed record CorregoRequest(
    [Required, StringLength(120, MinimumLength = 3)] string Nome,
    [Required, StringLength(80, MinimumLength = 2)] string Bairro,
    [Range(-90, 90)] double Latitude,
    [Range(-180, 180)] double Longitude,
    [Range(1, 10000)] double NivelAlertaCm,
    [Range(1, 10000)] double NivelCriticoCm);

/// <summary>Córrego monitorado.</summary>
public sealed record CorregoResponse(
    int Id,
    string Nome,
    string Bairro,
    double Latitude,
    double Longitude,
    double NivelAlertaCm,
    double NivelCriticoCm,
    DateTime CriadoEm);

/// <summary>Situação atual do córrego, calculada a partir da última leitura.</summary>
/// <param name="CorregoId">Id do córrego.</param>
/// <param name="Nome">Nome do córrego.</param>
/// <param name="NivelRisco">Risco da última leitura (NORMAL se ainda não houver leituras).</param>
/// <param name="UltimoNivelAguaCm">Último nível da água medido (cm).</param>
/// <param name="UltimaChuvaMmHora">Última chuva medida (mm/h).</param>
/// <param name="UltimaLeituraEm">Data/hora (UTC) da última leitura.</param>
/// <param name="UltimaLeitura">Última leitura completa.</param>
/// <param name="AlertasAtivos">Quantidade de alertas ativos do córrego.</param>
public sealed record StatusCorregoResponse(
    int CorregoId,
    string Nome,
    NivelRisco NivelRisco,
    double? UltimoNivelAguaCm,
    double? UltimaChuvaMmHora,
    DateTime? UltimaLeituraEm,
    LeituraResponse? UltimaLeitura,
    int AlertasAtivos);

/// <summary>Estatísticas das leituras de um córrego em uma janela de tempo.</summary>
public sealed record EstatisticasCorregoResponse(
    int CorregoId,
    int JanelaHoras,
    DateTime De,
    DateTime Ate,
    int TotalLeituras,
    double? MediaNivelAguaCm,
    double? MaximoNivelAguaCm,
    double? MinimoNivelAguaCm,
    double? MaximaChuvaMmHora,
    IReadOnlyList<EstatisticaPorRiscoResponse> PorNivelRisco);

/// <summary>Agrupamento das leituras por nível de risco.</summary>
public sealed record EstatisticaPorRiscoResponse(
    NivelRisco NivelRisco,
    int Quantidade,
    double MediaNivelAguaCm,
    double MaximoNivelAguaCm);
