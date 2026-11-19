namespace AlertaCorrego.Domain.Excecoes;

/// <summary>
/// Exceção base (abstrata) para erros de domínio. A API converte cada
/// especialização em um <c>ProblemDetails</c> com o status HTTP adequado.
/// </summary>
public abstract class DominioException : Exception
{
    protected DominioException(string mensagem)
        : base(mensagem)
    {
    }
}
