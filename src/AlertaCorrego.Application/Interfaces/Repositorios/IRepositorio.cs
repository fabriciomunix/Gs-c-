using AlertaCorrego.Domain.Comum;

namespace AlertaCorrego.Application.Interfaces.Repositorios;

/// <summary>Operações comuns de persistência de qualquer entidade.</summary>
public interface IRepositorio<TEntidade>
    where TEntidade : EntidadeBase
{
    /// <summary>Obtém a entidade (rastreada, pronta para alteração) pelo id.</summary>
    Task<TEntidade?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task AdicionarAsync(TEntidade entidade, CancellationToken cancellationToken = default);

    void Remover(TEntidade entidade);
}
