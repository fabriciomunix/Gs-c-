using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AlertaCorrego.Infrastructure.Persistencia;

/// <summary>
/// Contexto do EF Core. Também atua como Unit of Work da aplicação.
/// </summary>
public sealed class AppDbContext : DbContext, IUnidadeDeTrabalho
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Corrego> Corregos => Set<Corrego>();

    public DbSet<Sensor> Sensores => Set<Sensor>();

    public DbSet<Leitura> Leituras => Set<Leitura>();

    public DbSet<Morador> Moradores => Set<Morador>();

    public DbSet<Alerta> Alertas => Set<Alerta>();

    public Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default) =>
        SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as classes IEntityTypeConfiguration<T> da pasta Configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        MarcarDatasComoUtc(modelBuilder);
    }

    /// <summary>
    /// O SQLite não guarda o "Kind" do DateTime; ao ler, marcamos como UTC para que
    /// o JSON saia com o sufixo "Z".
    /// </summary>
    private static void MarcarDatasComoUtc(ModelBuilder modelBuilder)
    {
        var conversorUtc = new ValueConverter<DateTime, DateTime>(
            valor => valor,
            valor => DateTime.SpecifyKind(valor, DateTimeKind.Utc));

        var conversorUtcNulavel = new ValueConverter<DateTime?, DateTime?>(
            valor => valor,
            valor => valor.HasValue ? DateTime.SpecifyKind(valor.Value, DateTimeKind.Utc) : valor);

        foreach (var entidade in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var propriedade in entidade.GetProperties())
            {
                if (propriedade.ClrType == typeof(DateTime))
                {
                    propriedade.SetValueConverter(conversorUtc);
                }
                else if (propriedade.ClrType == typeof(DateTime?))
                {
                    propriedade.SetValueConverter(conversorUtcNulavel);
                }
            }
        }
    }
}
