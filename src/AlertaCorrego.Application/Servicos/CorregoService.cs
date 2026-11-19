using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Mapeamentos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Enums;
using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Application.Servicos;

/// <summary>Casos de uso de córregos: CRUD, status atual e estatísticas.</summary>
public sealed class CorregoService : ICorregoService
{
    public const int JanelaMaximaHoras = 24 * 30;

    private readonly ICorregoRepositorio _corregos;
    private readonly ILeituraRepositorio _leituras;
    private readonly IAlertaRepositorio _alertas;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
    private readonly TimeProvider _tempo;

    public CorregoService(
        ICorregoRepositorio corregos,
        ILeituraRepositorio leituras,
        IAlertaRepositorio alertas,
        IUnidadeDeTrabalho unidadeDeTrabalho,
        TimeProvider tempo)
    {
        _corregos = corregos;
        _leituras = leituras;
        _alertas = alertas;
        _unidadeDeTrabalho = unidadeDeTrabalho;
        _tempo = tempo;
    }

    public async Task<IReadOnlyList<CorregoResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var corregos = await _corregos.ListarAsync(cancellationToken);
        return corregos.Select(c => c.ParaResponse()).ToList();
    }

    public async Task<CorregoResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var corrego = await ObterEntidadeAsync(id, cancellationToken);
        return corrego.ParaResponse();
    }

    public async Task<CorregoResponse> CriarAsync(CorregoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var corrego = new Corrego(
            request.Nome,
            request.Bairro,
            request.Latitude,
            request.Longitude,
            request.NivelAlertaCm,
            request.NivelCriticoCm);

        await _corregos.AdicionarAsync(corrego, cancellationToken);
        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return corrego.ParaResponse();
    }

    public async Task<CorregoResponse> AtualizarAsync(int id, CorregoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var corrego = await ObterEntidadeAsync(id, cancellationToken);
        corrego.Atualizar(
            request.Nome,
            request.Bairro,
            request.Latitude,
            request.Longitude,
            request.NivelAlertaCm,
            request.NivelCriticoCm);

        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return corrego.ParaResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var corrego = await ObterEntidadeAsync(id, cancellationToken);
        _corregos.Remover(corrego);
        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task<StatusCorregoResponse> ObterStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var corrego = await ObterEntidadeAsync(id, cancellationToken);

        var ultimaLeitura = await _leituras.ObterUltimaPorCorregoAsync(id, cancellationToken);
        var alertasAtivos = await _alertas.ContarAtivosAsync(id, cancellationToken);

        return new StatusCorregoResponse(
            corrego.Id,
            corrego.Nome,
            ultimaLeitura?.NivelRisco ?? NivelRisco.Normal,
            ultimaLeitura?.NivelAguaCm,
            ultimaLeitura?.ChuvaMmHora,
            ultimaLeitura?.RegistradaEm,
            ultimaLeitura,
            alertasAtivos);
    }

    public async Task<EstatisticasCorregoResponse> ObterEstatisticasAsync(int id, int horas, CancellationToken cancellationToken = default)
    {
        if (horas is < 1 or > JanelaMaximaHoras)
        {
            throw new ValidacaoDominioException(
                nameof(horas),
                $"A janela de tempo deve estar entre 1 e {JanelaMaximaHoras} horas.");
        }

        var corrego = await ObterEntidadeAsync(id, cancellationToken);

        var ate = _tempo.GetUtcNow().UtcDateTime;
        var de = ate.AddHours(-horas);

        var grupos = await _leituras.ObterAgregadosPorRiscoAsync(corrego.Id, de, ate, cancellationToken);
        var total = grupos.Sum(g => g.Quantidade);

        double? media = total == 0 ? null : Math.Round(grupos.Sum(g => g.SomaNivelAguaCm) / total, 2);
        double? maximo = total == 0 ? null : grupos.Max(g => g.MaximoNivelAguaCm);
        double? minimo = total == 0 ? null : grupos.Min(g => g.MinimoNivelAguaCm);
        double? maximaChuva = total == 0 ? null : grupos.Max(g => g.MaximaChuvaMmHora);

        var porRisco = grupos
            .OrderBy(g => g.NivelRisco)
            .Select(g => new EstatisticaPorRiscoResponse(
                g.NivelRisco,
                g.Quantidade,
                Math.Round(g.SomaNivelAguaCm / g.Quantidade, 2),
                g.MaximoNivelAguaCm))
            .ToList();

        return new EstatisticasCorregoResponse(
            corrego.Id,
            horas,
            de,
            ate,
            total,
            media,
            maximo,
            minimo,
            maximaChuva,
            porRisco);
    }

    private async Task<Corrego> ObterEntidadeAsync(int id, CancellationToken cancellationToken) =>
        await _corregos.ObterPorIdAsync(id, cancellationToken)
        ?? throw new EntidadeNaoEncontradaException("Córrego", id);
}
