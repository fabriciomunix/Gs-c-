namespace AlertaCorrego.Application.Interfaces.Repositorios;

/// <summary>Unit of Work: confirma todas as alterações em uma única transação.</summary>
public interface IUnidadeDeTrabalho
{
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
