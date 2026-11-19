using AlertaCorrego.Application.Dtos;

namespace AlertaCorrego.Application.Interfaces;

public interface IMoradorService
{
    Task<IReadOnlyList<MoradorResponse>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default);

    Task<MoradorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<MoradorResponse> CriarAsync(MoradorRequest request, CancellationToken cancellationToken = default);

    Task<MoradorResponse> AtualizarAsync(int id, MoradorRequest request, CancellationToken cancellationToken = default);

    Task RemoverAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Moradores prioritários ordenados por urgência (acamado, cadeirante, idoso...).</summary>
    Task<IReadOnlyList<MoradorResponse>> ListarPrioritariosAsync(int? corregoId, CancellationToken cancellationToken = default);
}
