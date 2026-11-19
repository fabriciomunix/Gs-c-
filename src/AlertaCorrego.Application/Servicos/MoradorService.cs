using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Application.Mapeamentos;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Application.Servicos;

/// <summary>Casos de uso de moradores (dados simulados, nunca registrados em log).</summary>
public sealed class MoradorService : IMoradorService
{
    private readonly IMoradorRepositorio _moradores;
    private readonly ICorregoRepositorio _corregos;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public MoradorService(
        IMoradorRepositorio moradores,
        ICorregoRepositorio corregos,
        IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _moradores = moradores;
        _corregos = corregos;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public async Task<IReadOnlyList<MoradorResponse>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default)
    {
        var moradores = await _moradores.ListarAsync(corregoId, cancellationToken);
        return moradores.Select(m => m.ParaResponse()).ToList();
    }

    public async Task<MoradorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var morador = await ObterEntidadeAsync(id, cancellationToken);
        return morador.ParaResponse();
    }

    public async Task<MoradorResponse> CriarAsync(MoradorRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await GarantirCorregoExisteAsync(request.CorregoId, cancellationToken);

        var morador = new Morador(
            request.Nome,
            request.Telefone,
            request.Endereco,
            request.CorregoId,
            request.Prioritario,
            request.TipoNecessidade);

        await _moradores.AdicionarAsync(morador, cancellationToken);
        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);

        return morador.ParaResponse();
    }

    public async Task<MoradorResponse> AtualizarAsync(int id, MoradorRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var morador = await ObterEntidadeAsync(id, cancellationToken);
        await GarantirCorregoExisteAsync(request.CorregoId, cancellationToken);

        morador.Atualizar(
            request.Nome,
            request.Telefone,
            request.Endereco,
            request.CorregoId,
            request.Prioritario,
            request.TipoNecessidade);

        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
        return morador.ParaResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var morador = await ObterEntidadeAsync(id, cancellationToken);
        _moradores.Remover(morador);
        await _unidadeDeTrabalho.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MoradorResponse>> ListarPrioritariosAsync(int? corregoId, CancellationToken cancellationToken = default)
    {
        if (corregoId is int id)
        {
            await GarantirCorregoExisteAsync(id, cancellationToken);
        }

        var prioritarios = await _moradores.ListarPrioritariosAsync(corregoId, cancellationToken);

        return prioritarios
            .OrderByDescending(m => m.GrauPrioridade)
            .ThenBy(m => m.Nome, StringComparer.CurrentCulture)
            .Select(m => m.ParaResponse())
            .ToList();
    }

    private async Task<Morador> ObterEntidadeAsync(int id, CancellationToken cancellationToken) =>
        await _moradores.ObterPorIdAsync(id, cancellationToken)
        ?? throw new EntidadeNaoEncontradaException("Morador", id);

    private async Task GarantirCorregoExisteAsync(int corregoId, CancellationToken cancellationToken)
    {
        if (!await _corregos.ExisteAsync(corregoId, cancellationToken))
        {
            throw new EntidadeNaoEncontradaException("Córrego", corregoId);
        }
    }
}
