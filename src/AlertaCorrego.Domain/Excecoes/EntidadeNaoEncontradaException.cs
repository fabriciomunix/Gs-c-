namespace AlertaCorrego.Domain.Excecoes;

/// <summary>Recurso inexistente (HTTP 404).</summary>
public sealed class EntidadeNaoEncontradaException : DominioException
{
    public EntidadeNaoEncontradaException(string entidade, int id)
        : base($"{entidade} com id {id} não foi encontrado(a).")
    {
        Entidade = entidade;
        IdProcurado = id;
    }

    /// <summary>Nome da entidade procurada.</summary>
    public string Entidade { get; }

    /// <summary>Identificador procurado.</summary>
    public int IdProcurado { get; }
}
