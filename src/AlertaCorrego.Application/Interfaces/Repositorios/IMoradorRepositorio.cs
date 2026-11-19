using AlertaCorrego.Domain.Entidades;

namespace AlertaCorrego.Application.Interfaces.Repositorios;

public interface IMoradorRepositorio : IRepositorio<Morador>
{
    Task<IReadOnlyList<Morador>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Morador>> ListarPrioritariosAsync(int? corregoId, CancellationToken cancellationToken = default);
}
