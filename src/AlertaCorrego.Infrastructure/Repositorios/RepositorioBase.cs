using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Domain.Comum;
using AlertaCorrego.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace AlertaCorrego.Infrastructure.Repositorios;

/// <summary>
/// Implementação genérica (herdada pelos repositórios concretos) das operações comuns.
/// </summary>
public abstract class RepositorioBase<TEntidade> : IRepositorio<TEntidade>
    where TEntidade : EntidadeBase
{
    protected RepositorioBase(AppDbContext contexto)
    {
        Contexto = contexto;
    }

    protected AppDbContext Contexto { get; }

    protected DbSet<TEntidade> Conjunto => Contexto.Set<TEntidade>();

    public virtual Task<TEntidade?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        Conjunto.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task AdicionarAsync(TEntidade entidade, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entidade);
        await Conjunto.AddAsync(entidade, cancellationToken);
    }

    public virtual void Remover(TEntidade entidade)
    {
        ArgumentNullException.ThrowIfNull(entidade);
        Conjunto.Remove(entidade);
    }
}
