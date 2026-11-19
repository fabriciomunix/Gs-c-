using AlertaCorrego.Application.Dtos;

namespace AlertaCorrego.Application.Interfaces;

public interface ISensorService
{
    Task<IReadOnlyList<SensorResponse>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default);

    Task<SensorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<SensorResponse> CriarAsync(SensorRequest request, CancellationToken cancellationToken = default);
}
