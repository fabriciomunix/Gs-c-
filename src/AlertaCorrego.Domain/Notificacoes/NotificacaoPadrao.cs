using AlertaCorrego.Domain.Entidades;

namespace AlertaCorrego.Domain.Notificacoes;

/// <summary>Notificação para moradores sem necessidade de apoio para sair.</summary>
public sealed class NotificacaoPadrao : Notificacao
{
    public NotificacaoPadrao(Morador morador, Alerta alerta)
        : base(morador, alerta)
    {
    }

    public override string Tipo => "PADRAO";

    public override int Prioridade => 0;

    public override string MontarMensagem() =>
        $"{MensagemAlerta} Se puder, verifique se algum vizinho precisa de ajuda.";
}
