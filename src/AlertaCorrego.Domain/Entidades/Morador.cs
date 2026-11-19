using AlertaCorrego.Domain.Comum;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Domain.Entidades;

/// <summary>
/// Morador cadastrado às margens de um córrego. Dados SIMULADOS (sem CPF);
/// nunca devem ser gravados em log.
/// </summary>
public sealed class Morador : EntidadeBase
{
    public const int TamanhoMaximoNome = 120;
    public const int TamanhoMaximoTelefone = 13;
    public const int TamanhoMaximoEndereco = 200;

    private Morador()
    {
    }

    public Morador(
        string nome,
        string telefone,
        string endereco,
        int corregoId,
        bool prioritario,
        TipoNecessidade tipoNecessidade)
    {
        Atualizar(nome, telefone, endereco, corregoId, prioritario, tipoNecessidade);
    }

    public string Nome { get; private set; } = string.Empty;

    /// <summary>Telefone somente com dígitos (DDD + número, opcionalmente com DDI).</summary>
    public string Telefone { get; private set; } = string.Empty;

    public string Endereco { get; private set; } = string.Empty;

    public int CorregoId { get; private set; }

    /// <summary>Precisa de ajuda para sair de casa em uma emergência.</summary>
    public bool Prioritario { get; private set; }

    public TipoNecessidade TipoNecessidade { get; private set; }

    /// <summary>
    /// Grau usado para ordenar o atendimento (maior = mais urgente).
    /// Acamado &gt; Cadeirante &gt; Idoso &gt; Outra &gt; prioritário sem necessidade declarada.
    /// </summary>
    public int GrauPrioridade => TipoNecessidade switch
    {
        TipoNecessidade.Acamado => 4,
        TipoNecessidade.Cadeirante => 3,
        TipoNecessidade.Idoso => 2,
        TipoNecessidade.Outra => 1,
        _ => Prioritario ? 1 : 0
    };

    /// <summary>
    /// Atualiza os dados do morador. Quem declara uma necessidade especial
    /// passa a ser prioritário automaticamente.
    /// </summary>
    public void Atualizar(
        string nome,
        string telefone,
        string endereco,
        int corregoId,
        bool prioritario,
        TipoNecessidade tipoNecessidade)
    {
        if (!Enum.IsDefined(tipoNecessidade))
        {
            throw new ValidacaoDominioException(nameof(TipoNecessidade), "Tipo de necessidade inválido.");
        }

        Nome = Guarda.TextoObrigatorio(nome, nameof(Nome), TamanhoMaximoNome, tamanhoMinimo: 2);
        Telefone = NormalizarTelefone(telefone);
        Endereco = Guarda.TextoObrigatorio(endereco, nameof(Endereco), TamanhoMaximoEndereco, tamanhoMinimo: 5);
        CorregoId = Guarda.IdValido(corregoId, nameof(CorregoId));
        TipoNecessidade = tipoNecessidade;
        Prioritario = prioritario || tipoNecessidade != TipoNecessidade.Nenhuma;
    }

    private static string NormalizarTelefone(string? telefone)
    {
        var texto = Guarda.TextoObrigatorio(telefone, nameof(Telefone), 30);
        var digitos = new string(texto.Where(char.IsDigit).ToArray());

        if (digitos.Length is < 10 or > TamanhoMaximoTelefone)
        {
            throw new ValidacaoDominioException(
                nameof(Telefone),
                "O telefone deve conter DDD e número (10 a 13 dígitos).");
        }

        return digitos;
    }
}
