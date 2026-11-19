using System.ComponentModel.DataAnnotations;

namespace AlertaCorrego.Application.Dtos;

/// <summary>Dados para cadastrar um sensor.</summary>
/// <param name="Codigo">Código do hardware (ex.: ESP32-PB-02).</param>
/// <param name="CorregoId">Córrego onde o sensor foi instalado.</param>
/// <param name="Ativo">Se o sensor já está enviando leituras.</param>
/// <param name="InstaladoEm">Data/hora (UTC) da instalação; se omitida, usa a data atual.</param>
public sealed record SensorRequest(
    [Required, StringLength(40, MinimumLength = 3)] string Codigo,
    [Range(1, int.MaxValue)] int CorregoId,
    bool Ativo = true,
    DateTime? InstaladoEm = null);

/// <summary>Sensor instalado em um córrego.</summary>
public sealed record SensorResponse(
    int Id,
    string Codigo,
    int CorregoId,
    string? CorregoNome,
    bool Ativo,
    DateTime InstaladoEm,
    DateTime CriadoEm);
