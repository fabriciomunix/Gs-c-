using AlertaCorrego.Domain.Comum;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Domain.Entidades;

/// <summary>
/// Alerta gerado para um córrego quando uma leitura atinge ALERTA ou CRITICO.
/// </summary>
public sealed class Alerta : EntidadeBase
{
    public const int TamanhoMaximoMensagem = 500;

    private Alerta()
    {
    }

    public Alerta(int corregoId, NivelRisco nivelRisco, string mensagem, double nivelAguaCm)
    {
        if (!nivelRisco.ExigeAlerta())
        {
            throw new ValidacaoDominioException(
                nameof(NivelRisco),
                "Alertas só podem ser criados para os níveis ALERTA ou CRITICO.");
        }

        CorregoId = corregoId;
        NivelRisco = nivelRisco;
        Mensagem = Guarda.TextoObrigatorio(mensagem, nameof(Mensagem), TamanhoMaximoMensagem);
        NivelAguaCm = Guarda.Faixa(nivelAguaCm, 0, Leitura.NivelMaximoCm, nameof(NivelAguaCm));
        Ativo = true;
    }

    public int CorregoId { get; private set; }

    public NivelRisco NivelRisco { get; private set; }

    public string Mensagem { get; private set; } = string.Empty;

    /// <summary>Nível da água (cm) que disparou o alerta.</summary>
    public double NivelAguaCm { get; private set; }

    public bool Ativo { get; private set; }

    /// <summary>Data/hora (UTC) em que o alerta foi encerrado.</summary>
    public DateTime? EncerradoEm { get; private set; }

    /// <summary>
    /// Fábrica: cria o alerta de um córrego a partir de uma leitura já classificada.
    /// </summary>
    public static Alerta Gerar(Corrego corrego, Leitura leitura)
    {
        ArgumentNullException.ThrowIfNull(corrego);
        ArgumentNullException.ThrowIfNull(leitura);

        var mensagem = leitura.NivelRisco == NivelRisco.Critico
            ? $"RISCO CRÍTICO: {corrego.Nome} atingiu {leitura.NivelAguaCm:0.#} cm " +
              $"(limite crítico {corrego.NivelCriticoCm:0.#} cm). Saia agora das áreas baixas " +
              "e ajude vizinhos com mobilidade reduzida. Defesa Civil: 199."
            : $"ALERTA: {corrego.Nome} atingiu {leitura.NivelAguaCm:0.#} cm " +
              $"(limite de alerta {corrego.NivelAlertaCm:0.#} cm). Prepare-se para sair " +
              "e acompanhe as atualizações.";

        return new Alerta(corrego.Id, leitura.NivelRisco, mensagem, leitura.NivelAguaCm);
    }

    /// <summary>Encerra o alerta. Um alerta já encerrado não pode ser encerrado de novo.</summary>
    public void Encerrar(DateTime? encerradoEm = null)
    {
        if (!Ativo)
        {
            throw new RegraNegocioException("O alerta já está encerrado.");
        }

        Ativo = false;
        EncerradoEm = encerradoEm ?? DateTime.UtcNow;
    }
}
