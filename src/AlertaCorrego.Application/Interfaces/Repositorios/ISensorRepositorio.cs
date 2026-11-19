using AlertaCorrego.Domain.Entidades;

namespace AlertaCorrego.Application.Interfaces.Repositorios;

public interface ISensorRepositorio : IRepositorio<Sensor>
{
    Task<IReadOnlyList<Sensor>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default);

    Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken = default);
}
