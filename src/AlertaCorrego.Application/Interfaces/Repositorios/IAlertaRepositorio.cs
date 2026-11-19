using AlertaCorrego.Domain.Entidades;

namespace AlertaCorrego.Application.Interfaces.Repositorios;

public interface IAlertaRepositorio : IRepositorio<Alerta>
{
    Task<IReadOnlyList<Alerta>> ListarAsync(bool? ativos, int? corregoId, CancellationToken cancellationToken = default);

    Task<int> ContarAtivosAsync(int corregoId, CancellationToken cancellationToken = default);
}
