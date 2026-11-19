namespace AlertaCorrego.Domain.Excecoes;

/// <summary>Violação de invariante/validação de um campo (HTTP 400).</summary>
public sealed class ValidacaoDominioException : DominioException
{
    public ValidacaoDominioException(string campo, string mensagem)
        : base(mensagem)
    {
        Campo = campo;
    }

    /// <summary>Nome do campo inválido.</summary>
    public string Campo { get; }
}
