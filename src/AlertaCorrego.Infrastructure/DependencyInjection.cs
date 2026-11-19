using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Infrastructure.Persistencia;
using AlertaCorrego.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlertaCorrego.Infrastructure;

/// <summary>Registro da infraestrutura (EF Core + repositórios) no contêiner de DI.</summary>
public static class DependencyInjection
{
    public const string NomeConnectionString = "AlertaCorrego";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(NomeConnectionString)
            ?? throw new InvalidOperationException(
                $"A connection string '{NomeConnectionString}' não foi configurada no appsettings.json.");

        services.AddDbContext<AppDbContext>(opcoes => opcoes.UseSqlite(connectionString));

        // O próprio DbContext é a Unit of Work (mesma instância por requisição).
        services.AddScoped<IUnidadeDeTrabalho>(provedor => provedor.GetRequiredService<AppDbContext>());

        services.AddScoped<ICorregoRepositorio, CorregoRepositorio>();
        services.AddScoped<ISensorRepositorio, SensorRepositorio>();
        services.AddScoped<ILeituraRepositorio, LeituraRepositorio>();
        services.AddScoped<IMoradorRepositorio, MoradorRepositorio>();
        services.AddScoped<IAlertaRepositorio, AlertaRepositorio>();

        return services;
    }

    /// <summary>
    /// Cria o banco SQLite (com os dados de seed) caso ainda não exista.
    /// </summary>
    public static async Task InicializarBancoDeDadosAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await using var escopo = serviceProvider.CreateAsyncScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<AppDbContext>();
        await contexto.Database.EnsureCreatedAsync(cancellationToken);
    }
}
