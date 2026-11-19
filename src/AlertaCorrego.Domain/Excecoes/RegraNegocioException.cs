namespace AlertaCorrego.Domain.Excecoes;

/// <summary>Operação que conflita com o estado atual do domínio (HTTP 409).</summary>
public sealed class RegraNegocioException : DominioException
{
    public RegraNegocioException(string mensagem)
        : base(mensagem)
    {
    }
}
