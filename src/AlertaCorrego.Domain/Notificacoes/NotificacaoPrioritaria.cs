using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Domain.Notificacoes;

/// <summary>
/// Notificação para moradores prioritários (idosos, cadeirantes, acamados...):
/// é enviada primeiro e orienta o pedido de ajuda.
/// </summary>
public sealed class NotificacaoPrioritaria : Notificacao
{
    private readonly int _grauPrioridade;

    public NotificacaoPrioritaria(Morador morador, Alerta alerta)
        : base(morador, alerta)
    {
        TipoNecessidade = morador.TipoNecessidade;
        _grauPrioridade = morador.GrauPrioridade;
    }

    public TipoNecessidade TipoNecessidade { get; }

    public override string Tipo => "PRIORITARIA";

    public override int Prioridade => 10 + _grauPrioridade;

    public override string MontarMensagem() =>
        $"[PRIORIDADE] {MensagemAlerta} Você está na lista de apoio para saída: " +
        "toque em 'Pedir ajuda' no app ou ligue 199 (Defesa Civil).";
}
