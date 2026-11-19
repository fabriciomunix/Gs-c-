using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Domain.Notificacoes;

/// <summary>
/// Notificação (abstrata) enviada a um morador quando um alerta é gerado.
/// Cada especialização define sua prioridade e o texto da mensagem (polimorfismo).
/// </summary>
public abstract class Notificacao
{
    protected Notificacao(Morador morador, Alerta alerta)
    {
        ArgumentNullException.ThrowIfNull(morador);
        ArgumentNullException.ThrowIfNull(alerta);

        MoradorId = morador.Id;
        AlertaId = alerta.Id;
        NivelRisco = alerta.NivelRisco;
        MensagemAlerta = alerta.Mensagem;
    }

    public int MoradorId { get; }

    public int AlertaId { get; }

    public NivelRisco NivelRisco { get; }

    protected string MensagemAlerta { get; }

    /// <summary>Tipo da notificação (ex.: PADRAO, PRIORITARIA).</summary>
    public abstract string Tipo { get; }

    /// <summary>Ordem de envio: maior = enviar/atender primeiro.</summary>
    public abstract int Prioridade { get; }

    /// <summary>Monta o texto que será exibido no app do morador.</summary>
    public abstract string MontarMensagem();

    /// <summary>
    /// Fábrica polimórfica: moradores prioritários recebem uma <see cref="NotificacaoPrioritaria"/>,
    /// os demais uma <see cref="NotificacaoPadrao"/>.
    /// </summary>
    public static Notificacao Criar(Morador morador, Alerta alerta)
    {
        ArgumentNullException.ThrowIfNull(morador);

        return morador.Prioritario
            ? new NotificacaoPrioritaria(morador, alerta)
            : new NotificacaoPadrao(morador, alerta);
    }
}
