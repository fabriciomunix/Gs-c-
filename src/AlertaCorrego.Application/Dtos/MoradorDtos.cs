using System.ComponentModel.DataAnnotations;
using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Application.Dtos;

/// <summary>Dados (simulados) para cadastrar ou atualizar um morador.</summary>
/// <param name="Nome">Nome do morador.</param>
/// <param name="Telefone">Telefone com DDD (apenas dígitos são armazenados).</param>
/// <param name="Endereco">Endereço às margens do córrego.</param>
/// <param name="CorregoId">Córrego próximo à residência.</param>
/// <param name="Prioritario">Precisa de ajuda para sair (definido automaticamente se houver necessidade).</param>
/// <param name="TipoNecessidade">NENHUMA | IDOSO | CADEIRANTE | ACAMADO | OUTRA.</param>
public sealed record MoradorRequest(
    [Required, StringLength(120, MinimumLength = 2)] string Nome,
    [Required, StringLength(30, MinimumLength = 10)] string Telefone,
    [Required, StringLength(200, MinimumLength = 5)] string Endereco,
    [Range(1, int.MaxValue)] int CorregoId,
    bool Prioritario,
    TipoNecessidade TipoNecessidade);

/// <summary>Morador cadastrado.</summary>
public sealed record MoradorResponse(
    int Id,
    string Nome,
    string Telefone,
    string Endereco,
    int CorregoId,
    bool Prioritario,
    TipoNecessidade TipoNecessidade,
    DateTime CriadoEm);
