using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace AlertaCorrego.Infrastructure.Repositorios;

public sealed class MoradorRepositorio : RepositorioBase<Morador>, IMoradorRepositorio
{
    public MoradorRepositorio(AppDbContext contexto)
        : base(contexto)
    {
    }

    public async Task<IReadOnlyList<Morador>> ListarAsync(int? corregoId, CancellationToken cancellationToken = default) =>
        await Filtrar(corregoId)
            .OrderBy(m => m.Nome)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Morador>> ListarPrioritariosAsync(int? corregoId, CancellationToken cancellationToken = default) =>
        await Filtrar(corregoId)
            .Where(m => m.Prioritario)
            .OrderBy(m => m.Nome)
            .ToListAsync(cancellationToken);

    private IQueryable<Morador> Filtrar(int? corregoId)
    {
        IQueryable<Morador> consulta = Conjunto.AsNoTracking();

        if (corregoId is int id)
        {
            consulta = consulta.Where(m => m.CorregoId == id);
        }

        return consulta;
    }
}
