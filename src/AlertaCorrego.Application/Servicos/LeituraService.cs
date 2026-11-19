using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Mapeamentos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Excecoes;
using AlertaCorrego.Domain.Notificacoes;
using AlertaCorrego.Domain.Risco;
using Microsoft.Extensions.Logging;

namespace AlertaCorrego.Application.Servicos;

/// <summary>
/// Caso de uso central: registrar a leitura do sensor, classificar o risco,
/// gerar o alerta e identificar quem precisa de ajuda para sair.
/// </summary>
public sealed class LeituraService : ILeituraService
{
    public const int LimiteMaximo = 500;

    private readonly ISensorRepositorio _sensores;
    private readonly ICorregoRepositorio _corregos;
    private readonly ILeituraRepositorio _leituras;
    private readonly IAlertaRepositorio _alertas;
    private readonly IMoradorRepositorio _moradores;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
    private readonly IClassificadorRisco _classificador;
    private readonly TimeProvider _tempo;
    private readonly ILogger<LeituraService> _logger;

    public LeituraService(
        ISensorRepositorio sensores,
        ICorregoRepositorio corregos,
        ILeituraRepositorio leituras,
        IAlertaRepositorio alertas,
        IMoradorRepositorio moradores,
        IUnidadeDeTrabalho unidadeDeTrabalho,
        IClassificadorRisco classificador,
        TimeProvider tempo,
        ILogger<LeituraService> logger)
    {
        _sensores = sensores;
        _corregos = corregos;
        _leituras = leituras;
        _alertas = alertas;
        _moradores = moradores;
        _unidadeDeTrabalho = unidadeDeTrabalho;
        _classificador = classificador;
        _tempo = tempo;
        _logger = logger;
    }

    public async Task<RegistroLeituraResponse> RegistrarAsync(LeituraRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sensor = await _sensores.ObterPorIdAsync(request.SensorId, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException("Sensor", request.SensorId);

        var corrego = await _corregos.ObterPorIdAsync(sensor.CorregoId, cancellationToken)
            ?? throw new EntidadeNaoEncontradaException("Córrego", sensor.CorregoId);

        var registradaEm = request.RegistradaEm?.ToUniversalTime() ?? _tempo.GetUtcNow().UtcDateTime;

        // Regras de domínio: sensor ativo, valores válidos e classificação de risco.
        var leitura = sensor.RegistrarLeitura(request.NivelAguaCm, request.ChuvaMmHora, registradaEm);
        var nivelRisco = leitura.ClassificarRisco(corrego, _classificador);
        await _leituras.AdicionarAsync(leitura, cancellationToken);

        Alerta? alerta = null;
        IReadOnlyList<Morador> moradoresDoCorrego = Array.Empty<Morador>();

        if (leitura.ExigeAlerta)
        {
            alerta = Alerta.Gerar(corrego, leitura);
            await _alertas.AdicionarAsync(alerta, cancellationToken);
            moradoresDoCorrego = await _moradores.ListarAsync(corrego.Id, cancellationToken);
        }

        // Uma única transação para leitura + alerta.
        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        var prioritarios = moradoresDoCorrego
            .Where(m => m.Prioritario)
            .OrderByDescending(m => m.GrauPrioridade)
            .ThenBy(m => m.Nome, StringComparer.CurrentCulture)
            .Select(m => m.ParaResponse())
            .ToList();

        var notificacoes = alerta is null
            ? new List<NotificacaoResponse>()
            : MontarNotificacoes(alerta, moradoresDoCorrego);

        // Apenas identificadores e contagens no log: nenhum dado pessoal de morador.
        _logger.LogInformation(
            "Leitura {LeituraId} do sensor {SensorId} classificada como {NivelRisco} (córrego {CorregoId}).",
            leitura.Id,
            sensor.Id,
            nivelRisco,
            corrego.Id);

        if (alerta is not null)
        {
            _logger.LogWarning(
                "Alerta {AlertaId} gerado para o córrego {CorregoId} ({NivelRisco}); {QuantidadePrioritarios} morador(es) prioritário(s) identificado(s).",
                alerta.Id,
                corrego.Id,
                alerta.NivelRisco,
                prioritarios.Count);
        }

        return new RegistroLeituraResponse(
            leitura.ParaResponse(corrego.Id),
            nivelRisco,
            alerta?.ParaResponse(),
            prioritarios,
            notificacoes);
    }

    public async Task<IReadOnlyList<LeituraResponse>> ListarAsync(int? corregoId, int limite, CancellationToken cancellationToken = default)
    {
        if (limite is < 1 or > LimiteMaximo)
        {
            throw new ValidacaoDominioException(nameof(limite), $"O limite deve estar entre 1 e {LimiteMaximo}.");
        }

        return await _leituras.ListarAsync(corregoId, limite, cancellationToken);
    }

    private static List<NotificacaoResponse> MontarNotificacoes(Alerta alerta, IEnumerable<Morador> moradores) =>
        moradores
            .Select(m => Notificacao.Criar(m, alerta)) // polimorfismo: Padrao x Prioritaria
            .OrderByDescending(n => n.Prioridade)
            .Select(n => n.ParaResponse())
            .ToList();
}
