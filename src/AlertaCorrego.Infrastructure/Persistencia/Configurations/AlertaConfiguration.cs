using AlertaCorrego.Domain.Entidades;
using AlertaCorrego.Infrastructure.Persistencia.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertaCorrego.Infrastructure.Persistencia.Configurations;

public sealed class AlertaConfiguration : IEntityTypeConfiguration<Alerta>
{
    public void Configure(EntityTypeBuilder<Alerta> builder)
    {
        builder.ToTable("Alertas");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Mensagem).IsRequired().HasMaxLength(Alerta.TamanhoMaximoMensagem);
        builder.Property(a => a.NivelAguaCm).IsRequired();
        builder.Property(a => a.Ativo).IsRequired();
        builder.Property(a => a.CriadoEm).IsRequired();
        builder.Property(a => a.EncerradoEm);
        builder.Property(a => a.NivelRisco)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(a => new { a.CorregoId, a.Ativo });

        // Corrego 1-N Alerta
        builder.HasOne<Corrego>()
            .WithMany()
            .HasForeignKey(a => a.CorregoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(DadosIniciais.Alertas);
    }
}
