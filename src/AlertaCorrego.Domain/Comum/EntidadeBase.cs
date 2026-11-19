namespace AlertaCorrego.Domain.Comum;

/// <summary>
/// Classe base abstrata de todas as entidades do domínio.
/// Centraliza a identidade (<see cref="Id"/>) e a data de criação (<see cref="CriadoEm"/>).
/// </summary>
public abstract class EntidadeBase
{
    protected EntidadeBase()
    {
        CriadoEm = DateTime.UtcNow;
    }

    /// <summary>Identificador gerado pelo banco de dados.</summary>
    public int Id { get; protected set; }

    /// <summary>Data/hora (UTC) em que a entidade foi criada.</summary>
    public DateTime CriadoEm { get; protected set; }
}
