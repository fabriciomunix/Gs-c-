using System.ComponentModel.DataAnnotations;
using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Application.Dtos;

/// <summary>Leitura enviada pelo sensor ESP32.</summary>
/// <param name="SensorId">Id do sensor que realizou a medição.</param>
/// <param name="NivelAguaCm">Nível da água em centímetros.</param>
/// <param name="ChuvaMmHora">Intensidade da chuva em mm/h.</param>
/// <param name="RegistradaEm">Data/hora (UTC) da medição; se omitida, usa a hora do servidor.</param>
public sealed record LeituraRequest(
    [Range(1, int.MaxValue)] int SensorId,
    [Range(0, 10000)] double NivelAguaCm,
    [Range(0, 500)] double ChuvaMmHora,
    DateTime? RegistradaEm = null);

/// <summary>Leitura registrada.</summary>
public sealed record LeituraResponse(
    int Id,
    int SensorId,
    int CorregoId,
    double NivelAguaCm,
    double ChuvaMmHora,
    NivelRisco NivelRisco,
    DateTime RegistradaEm);

/// <summary>Notificação gerada para um morador quando há alerta.</summary>
public sealed record NotificacaoResponse(
    int MoradorId,
    string Tipo,
    int Prioridade,
    string Mensagem);

/// <summary>
/// Resultado do registro de uma leitura: leitura, risco, alerta gerado (se ALERTA/CRITICO)
/// e moradores prioritários que precisam de ajuda para sair.
/// </summary>
public sealed record RegistroLeituraResponse(
    LeituraResponse Leitura,
    NivelRisco NivelRisco,
    AlertaResponse? Alerta,
    IReadOnlyList<MoradorResponse> MoradoresPrioritarios,
    IReadOnlyList<NotificacaoResponse> Notificacoes);
