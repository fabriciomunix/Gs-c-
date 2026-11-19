using AlertaCorrego.Application.Dtos;

namespace AlertaCorrego.Application.Interfaces;

public interface IAlertaService
{
    Task<IReadOnlyList<AlertaResponse>> ListarAsync(bool? ativos, int? corregoId, CancellationToken cancellationToken = default);

    Task<AlertaResponse> EncerrarAsync(int id, CancellationToken cancellationToken = default);
}
