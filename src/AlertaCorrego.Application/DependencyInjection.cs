using AlertaCorrego.Application.Interfaces;
using AlertaCorrego.Application.Servicos;
using AlertaCorrego.Domain.Risco;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlertaCorrego.Application;

/// <summary>Registro dos serviços da camada de aplicação no contêiner de DI.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IClassificadorRisco, ClassificadorRiscoPadrao>();

        services.AddScoped<ICorregoService, CorregoService>();
        services.AddScoped<ISensorService, SensorService>();
        services.AddScoped<ILeituraService, LeituraService>();
        services.AddScoped<IMoradorService, MoradorService>();
        services.AddScoped<IAlertaService, AlertaService>();

        return services;
    }
}
