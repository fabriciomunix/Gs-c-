using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertaCorrego.Infrastructure.Persistencia.Configurations;

public sealed class SensorConfiguration : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.ToTable("Sensores");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Codigo).IsRequired().HasMaxLength(Sensor.TamanhoMaximoCodigo);
        builder.Property(s => s.Ativo).IsRequired();
        builder.Property(s => s.InstaladoEm).IsRequired();
        builder.Property(s => s.CriadoEm).IsRequired();

        builder.HasIndex(s => s.Codigo).IsUnique();

        // Corrego 1-N Sensor
        builder.HasOne(s => s.Corrego)
            .WithMany()
            .HasForeignKey(s => s.CorregoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(DadosIniciais.Sensores);
    }
}
