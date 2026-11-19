using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertaCorrego.Infrastructure.Persistencia.Configurations;

public sealed class MoradorConfiguration : IEntityTypeConfiguration<Morador>
{
    public void Configure(EntityTypeBuilder<Morador> builder)
    {
        builder.ToTable("Moradores");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nome).IsRequired().HasMaxLength(Morador.TamanhoMaximoNome);
        builder.Property(m => m.Telefone).IsRequired().HasMaxLength(Morador.TamanhoMaximoTelefone);
        builder.Property(m => m.Endereco).IsRequired().HasMaxLength(Morador.TamanhoMaximoEndereco);
        builder.Property(m => m.Prioritario).IsRequired();
        builder.Property(m => m.CriadoEm).IsRequired();
        builder.Property(m => m.TipoNecessidade)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Ignore(m => m.GrauPrioridade);

        builder.HasIndex(m => new { m.CorregoId, m.Prioritario });

        // Corrego 1-N Morador
        builder.HasOne<Corrego>()
            .WithMany()
            .HasForeignKey(m => m.CorregoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(DadosIniciais.Moradores);
    }
}
