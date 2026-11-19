using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace AlertaCorrego.Infrastructure.Repositorios;

public sealed class AlertaRepositorio : RepositorioBase<Alerta>, IAlertaRepositorio
{
    public AlertaRepositorio(AppDbContext contexto)
        : base(contexto)
    {
    }

    public async Task<IReadOnlyList<Alerta>> ListarAsync(bool? ativos, int? corregoId, CancellationToken cancellationToken = default)
    {
        IQueryable<Alerta> consulta = Conjunto.AsNoTracking();

        if (ativos is bool somenteAtivos)
        {
            consulta = consulta.Where(a => a.Ativo == somenteAtivos);
        }

        if (corregoId is int id)
        {
            consulta = consulta.Where(a => a.CorregoId == id);
        }

        return await consulta
            .OrderByDescending(a => a.CriadoEm)
            .ThenByDescending(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<int> ContarAtivosAsync(int corregoId, CancellationToken cancellationToken = default) =>
        Conjunto.CountAsync(a => a.CorregoId == corregoId && a.Ativo, cancellationToken);
}
