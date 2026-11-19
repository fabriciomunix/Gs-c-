using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Notificacoes;

namespace AlertaCorrego.Application.Mapeamentos;

/// <summary>Conversões entidade → DTO (mapeamento manual, sem bibliotecas externas).</summary>
public static class MapeamentoExtensions
{
    public static CorregoResponse ParaResponse(this Corrego corrego) => new(
        corrego.Id,
        corrego.Nome,
        corrego.Bairro,
        corrego.Latitude,
        corrego.Longitude,
        corrego.NivelAlertaCm,
        corrego.NivelCriticoCm,
        corrego.CriadoEm);

    public static SensorResponse ParaResponse(this Sensor sensor) => new(
        sensor.Id,
        sensor.Codigo,
        sensor.CorregoId,
        sensor.Corrego?.Nome,
        sensor.Ativo,
        sensor.InstaladoEm,
        sensor.CriadoEm);

    public static LeituraResponse ParaResponse(this Leitura leitura, int corregoId) => new(
        leitura.Id,
        leitura.SensorId,
        corregoId,
        leitura.NivelAguaCm,
        leitura.ChuvaMmHora,
        leitura.NivelRisco,
        leitura.RegistradaEm);

    public static MoradorResponse ParaResponse(this Morador morador) => new(
        morador.Id,
        morador.Nome,
        morador.Telefone,
        morador.Endereco,
        morador.CorregoId,
        morador.Prioritario,
        morador.TipoNecessidade,
        morador.CriadoEm);

    public static AlertaResponse ParaResponse(this Alerta alerta) => new(
        alerta.Id,
        alerta.CorregoId,
        alerta.NivelRisco,
        alerta.Mensagem,
        alerta.NivelAguaCm,
        alerta.CriadoEm,
        alerta.Ativo,
        alerta.EncerradoEm);

    public static NotificacaoResponse ParaResponse(this Notificacao notificacao) => new(
        notificacao.MoradorId,
        notificacao.Tipo,
        notificacao.Prioridade,
        notificacao.MontarMensagem());
}
