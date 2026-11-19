using AlertaCorrego.Application.Dtos;

namespace AlertaCorrego.Application.Interfaces;

public interface ICorregoService
{
    Task<IReadOnlyList<CorregoResponse>> ListarAsync(CancellationToken cancellationToken = default);

    Task<CorregoResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CorregoResponse> CriarAsync(CorregoRequest request, CancellationToken cancellationToken = default);

    Task<CorregoResponse> AtualizarAsync(int id, CorregoRequest request, CancellationToken cancellationToken = default);

    Task RemoverAsync(int id, CancellationToken cancellationToken = default);

    Task<StatusCorregoResponse> ObterStatusAsync(int id, CancellationToken cancellationToken = default);

    Task<EstatisticasCorregoResponse> ObterEstatisticasAsync(int id, int horas, CancellationToken cancellationToken = default);
}
