using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertaCorrego.Infrastructure.Persistencia.Configurations;

public sealed class LeituraConfiguration : IEntityTypeConfiguration<Leitura>
{
    public void Configure(EntityTypeBuilder<Leitura> builder)
    {
        builder.ToTable("Leituras");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.NivelAguaCm).IsRequired();
        builder.Property(l => l.ChuvaMmHora).IsRequired();
        builder.Property(l => l.RegistradaEm).IsRequired();
        builder.Property(l => l.CriadoEm).IsRequired();
        builder.Property(l => l.NivelRisco)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Ignore(l => l.ExigeAlerta);

        // Índice para "últimas leituras" e estatísticas por período.
        builder.HasIndex(l => new { l.SensorId, l.RegistradaEm });

        // Sensor 1-N Leitura
        builder.HasOne(l => l.Sensor)
            .WithMany()
            .HasForeignKey(l => l.SensorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(DadosIniciais.Leituras);
    }
}
