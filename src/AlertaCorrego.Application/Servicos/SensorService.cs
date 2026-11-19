using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Mapeamentos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Application.Servicos;

/// <summary>Casos de uso de sensores.</summary>
public sealed class SensorService : ISensorService
{
    private readonly ISensorRepositorio _sensores;
    private readonly ICorregoRepositorio _corregos;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
    private readonly TimeProvider _tempo;

    public SensorService(
        ISensorRepositorio sensores,
        ICorregoRepositorio corregos,
        IUnidadeDeTrabalho unidadeDeTrabalho,
        TimeProvider tempo)
    {
        _sensores = sensores;
        _corregos = corregos;
        _unidadeDeTrabalho = unidadeDeTrabalho;
        _tempo = tempo;
    }

    public async Task<IReadOnlyList<SensorResponse>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default)
    {
        var sensores = await _sensores.ListarAsync(corregoId, cancellationToken);
        return sensores.Select(s => s.ParaResponse()).ToList();
    }

    public async Task<SensorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sensor = await _sensores.ObterPorIdAsync(id, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException("Sensor", id);

        return sensor.ParaResponse();
    }

    public async Task<SensorResponse> CriarAsync(SensorRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Carregar o córrego (rastreado) permite ao EF preencher a navegação Sensor.Corrego.
        _ = await _corregos.ObterPorIdAsync(request.CorregoId, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException("Córrego", request.CorregoId);

        var instaladoEm = request.InstaladoEm?.ToUniversalTime() ?? _tempo.GetUtcNow().UtcDateTime;
        var sensor = new Sensor(request.Codigo, request.CorregoId, instaladoEm, request.Ativo);

        if (await _sensores.ExisteCodigoAsync(sensor.Codigo, cancellationToken))
        {
            throw new RegraNegocioException($"Já existe um sensor com o código {sensor.Codigo}.");
        }

        await _sensores.AdicionarAsync(sensor, cancellationToken);
        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return sensor.ParaResponse();
    }
}
