using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces.Repositorios;
using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace AlertaCorrego.Infrastructure.Repositorios;

public sealed class LeituraRepositorio : RepositorioBase<Leitura>, ILeituraRepositorio
{
    public LeituraRepositorio(AppDbContext contexto)
        : base(contexto)
    {
    }

    public async Task<IReadOnlyList<LeituraResponse>> ListarAsync(int? corregoId, int limite, CancellationToken cancellationToken = default)
    {
        IQueryable<Leitura> consulta = Conjunto.AsNoTracking();

        if (corregoId is int id)
        {
            consulta = consulta.Where(l => l.Sensor.CorregoId == id);
        }

        // Projeção (Select) direto para o DTO: o SQL traz apenas as colunas necessárias.
        return await consulta
            .OrderByDescending(l => l.RegistradaEm)
            .ThenByDescending(l => l.Id)
            .Take(limite)
            .Select(l => new LeituraResponse(
                l.Id,
                l.SensorId,
                l.Sensor.CorregoId,
                l.NivelAguaCm,
                l.ChuvaMmHora,
                l.NivelRisco,
                l.RegistradaEm))
            .ToListAsync(cancellationToken);
    }

    public Task<LeituraResponse?> ObterUltimaPorCorregoAsync(int corregoId, CancellationToken cancellationToken = default) =>
        Conjunto
            .AsNoTracking()
            .Where(l => l.Sensor.CorregoId == corregoId)
            .OrderByDescending(l => l.RegistradaEm)
            .ThenByDescending(l => l.Id)
            .Select(l => new LeituraResponse(
                l.Id,
                l.SensorId,
                l.Sensor.CorregoId,
                l.NivelAguaCm,
                l.ChuvaMmHora,
                l.NivelRisco,
                l.RegistradaEm))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<AgregadoLeituras>> ObterAgregadosPorRiscoAsync(
        int corregoId,
        DateTime de,
        DateTime ate,
        CancellationToken cancellationToken = default)
    {
        // GroupBy traduzido para SQL (GROUP BY NivelRisco) com COUNT/SUM/MAX/MIN.
        var grupos = await Conjunto
            .AsNoTracking()
            .Where(l => l.Sensor.CorregoId == corregoId && l.RegistradaEm >= de && l.RegistradaEm <= ate)
            .GroupBy(l => l.NivelRisco)
            .Select(g => new
            {
                NivelRisco = g.Key,
                Quantidade = g.Count(),
                SomaNivel = g.Sum(l => l.NivelAguaCm),
                MaximoNivel = g.Max(l => l.NivelAguaCm),
                MinimoNivel = g.Min(l => l.NivelAguaCm),
                MaximaChuva = g.Max(l => l.ChuvaMmHora)
            })
            .ToListAsync(cancellationToken);

        return grupos
            .Select(g => new AgregadoLeituras(
                g.NivelRisco,
                g.Quantidade,
                g.SomaNivel,
                g.MaximoNivel,
                g.MinimoNivel,
                g.MaximaChuva))
            .ToList();
    }
}
