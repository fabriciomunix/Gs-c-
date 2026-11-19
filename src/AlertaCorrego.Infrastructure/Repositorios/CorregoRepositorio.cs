using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace AlertaCorrego.Infrastructure.Repositorios;

public sealed class CorregoRepositorio : RepositorioBase<Corrego>, ICorregoRepositorio
{
    public CorregoRepositorio(AppDbContext contexto)
        : base(contexto)
    {
    }

    public async Task<IReadOnlyList<Corrego>> ListarAsync(CancellationToken cancellationToken = default) =>
        await Conjunto
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);

    public Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default) =>
        Conjunto.AnyAsync(c => c.Id == id, cancellationToken);
}
