using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Mapeamentos;
using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Application.Servicos;

/// <summary>Casos de uso de alertas.</summary>
public sealed class AlertaService : IAlertaService
{
    private readonly IAlertaRepositorio _alertas;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
    private readonly TimeProvider _tempo;

    public AlertaService(IAlertaRepositorio alertas, IUnidadeDeTrabalho unidadeDeTrabalho, TimeProvider tempo)
    {
        _alertas = alertas;
        _unidadeDeTrabalho = unidadeDeTrabalho;
        _tempo = tempo;
    }

    public async Task<IReadOnlyList<AlertaResponse>> ListarAsync(bool? ativos, int? corregoId, CancellationToken cancellationToken = default)
    {
        var alertas = await _alertas.ListarAsync(ativos, corregoId, cancellationToken);
        return alertas.Select(a => a.ParaResponse()).ToList();
    }

    public async Task<AlertaResponse> EncerrarAsync(int id, CancellationToken cancellationToken = default)
    {
        var alerta = await _alertas.ObterPorIdAsync(id, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException("Alerta", id);

        alerta.Encerrar(_tempo.GetUtcNow().UtcDateTime);
        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return alerta.ParaResponse();
    }
}
