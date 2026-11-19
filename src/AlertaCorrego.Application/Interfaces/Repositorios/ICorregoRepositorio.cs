using AlertaCorrego.Domain.Entidades;

namespace AlertaCorrego.Application.Interfaces.Repositorios;

public interface ICorregoRepositorio : IRepositorio<Corrego>
{
    Task<IReadOnlyList<Corrego>> ListarAsync(CancellationToken cancellationToken = default);

    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default);
}
