using AlertaCorrego.Domain.Comum;
using AlertaCorrego.Domain.Excecoes;

namespace AlertaCorrego.Domain.Entidades;

/// <summary>
/// Sensor ESP32 instalado em um córrego, que mede o nível da água e a chuva.
/// </summary>
public sealed class Sensor : EntidadeBase
{
    public const int TamanhoMaximoCodigo = 40;

    private Sensor()
    {
    }

    public Sensor(string codigo, int corregoId, DateTime instaladoEm, bool ativo = true)
    {
        Codigo = Guarda.TextoObrigatorio(codigo, nameof(Codigo), TamanhoMaximoCodigo, tamanhoMinimo: 3)
            .ToUpperInvariant();
        CorregoId = Guarda.IdValido(corregoId, nameof(CorregoId));
        InstaladoEm = instaladoEm;
        Ativo = ativo;
    }

    /// <summary>Código de identificação do hardware (ex.: ESP32-PB-01).</summary>
    public string Codigo { get; private set; } = string.Empty;

    public int CorregoId { get; private set; }

    /// <summary>Córrego onde o sensor está instalado (navegação do EF Core).</summary>
    public Corrego Corrego { get; private set; } = null!;

    public bool Ativo { get; private set; }

    public DateTime InstaladoEm { get; private set; }

    public void Ativar() => Ativo = true;

    public void Desativar() => Ativo = false;

    /// <summary>
    /// Cria uma nova leitura deste sensor. Sensores inativos não podem registrar leituras.
    /// A persistência é responsabilidade do repositório de leituras.
    /// </summary>
    public Leitura RegistrarLeitura(double nivelAguaCm, double chuvaMmHora, DateTime registradaEm)
    {
        if (!Ativo)
        {
            throw new RegraNegocioException($"O sensor {Codigo} está inativo e não pode registrar leituras.");
        }

        return new Leitura(Id, nivelAguaCm, chuvaMmHora, registradaEm);
    }
}
