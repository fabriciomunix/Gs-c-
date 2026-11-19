using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Application.Dtos;

/// <summary>Alerta de risco de um córrego.</summary>
public sealed record AlertaResponse(
    int Id,
    int CorregoId,
    NivelRisco NivelRisco,
    string Mensagem,
    double NivelAguaCm,
    DateTime CriadoEm,
    bool Ativo,
    DateTime? EncerradoEm);
