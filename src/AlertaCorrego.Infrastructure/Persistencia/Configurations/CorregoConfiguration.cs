using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertaCorrego.Infrastructure.Persistencia.Configurations;

public sealed class CorregoConfiguration : IEntityTypeConfiguration<Corrego>
{
    public void Configure(EntityTypeBuilder<Corrego> builder)
    {
        builder.ToTable("Corregos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome).IsRequired().HasMaxLength(Corrego.TamanhoMaximoNome);
        builder.Property(c => c.Bairro).IsRequired().HasMaxLength(Corrego.TamanhoMaximoBairro);
        builder.Property(c => c.Latitude).IsRequired();
        builder.Property(c => c.Longitude).IsRequired();
        builder.Property(c => c.NivelAlertaCm).IsRequired();
        builder.Property(c => c.NivelCriticoCm).IsRequired();
        builder.Property(c => c.CriadoEm).IsRequired();

        builder.HasIndex(c => c.Nome);

        builder.HasData(DadosIniciais.Corregos);
    }
}
