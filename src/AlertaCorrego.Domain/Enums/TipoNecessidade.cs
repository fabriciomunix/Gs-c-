namespace AlertaCorrego.Domain.Enums;

/// <summary>
/// Necessidade especial do morador para sair de casa em uma emergência.
/// Serializado na API como NENHUMA | IDOSO | CADEIRANTE | ACAMADO | OUTRA.
/// </summary>
public enum TipoNecessidade
{
    /// <summary>Sem necessidade especial.</summary>
    Nenhuma = 0,

    /// <summary>Pessoa idosa.</summary>
    Idoso = 1,

    /// <summary>Pessoa em cadeira de rodas.</summary>
    Cadeirante = 2,

    /// <summary>Pessoa acamada.</summary>
    Acamado = 3,

    /// <summary>Outra necessidade (gestante, PcD visual etc.).</summary>
    Outra = 4
}
