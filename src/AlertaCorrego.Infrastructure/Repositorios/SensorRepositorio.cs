using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace AlertaCorrego.Infrastructure.Repositorios;

public sealed class SensorRepositorio : RepositorioBase<Sensor>, ISensorRepositorio
{
    public SensorRepositorio(AppDbContext contexto)
        : base(contexto)
    {
    }

    /// <summary>Inclui o córrego para que a resposta traga o nome dele.</summary>
    public override Task<Sensor?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        Conjunto
            .Include(s => s.Corrego)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Sensor>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default)
    {
        IQueryable<Sensor> consulta = Conjunto
            .AsNoTracking()
            .Include(s => s.Corrego);

        if (corregoId is int id)
        {
            consulta = consulta.Where(s => s.CorregoId == id);
        }

        return await consulta
            .OrderBy(s => s.Codigo)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExisteCodigoAsync(string codigo, CancellationToken cancellationToken = default) =>
        Conjunto.AnyAsync(s => s.Codigo == codigo, cancellationToken);
}
