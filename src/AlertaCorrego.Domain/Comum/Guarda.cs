using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Domain.Comum;

/// <summary>
/// Cláusulas de guarda reutilizadas pelas entidades para validar invariantes.
/// As mensagens nunca incluem o valor recebido, para não expor dados pessoais.
/// </summary>
internal static class Guarda
{
    public static string TextoObrigatorio(string? valor, string campo, int tamanhoMaximo, int tamanhoMinimo = 1)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ValidacaoDominioException(campo, $"O campo '{campo}' é obrigatório.");
        }

        var texto = valor.Trim();
        if (texto.Length < tamanhoMinimo || texto.Length > tamanhoMaximo)
        {
            throw new ValidacaoDominioException(
                campo,
                $"O campo '{campo}' deve ter entre {tamanhoMinimo} e {tamanhoMaximo} caracteres.");
        }

        return texto;
    }

    public static double Faixa(double valor, double minimo, double maximo, string campo)
    {
        if (double.IsNaN(valor) || double.IsInfinity(valor) || valor < minimo || valor > maximo)
        {
            throw new ValidacaoDominioException(
                campo,
                $"O campo '{campo}' deve estar entre {minimo} e {maximo}.");
        }

        return valor;
    }

    public static int IdValido(int id, string campo)
    {
        if (id <= 0)
        {
            throw new ValidacaoDominioException(campo, $"O campo '{campo}' deve ser um identificador válido (maior que zero).");
        }

        return id;
    }
}
