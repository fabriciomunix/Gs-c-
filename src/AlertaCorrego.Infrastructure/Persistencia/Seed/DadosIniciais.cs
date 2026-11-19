using AlertaCorrego.Domain.Enums;

namespace AlertaCorrego.Infrastructure.Persistencia.Seed;

/// <summary>
/// Dados de seed (FICHA): 2 córregos, 1 sensor cada, 6 moradores FICTÍCIOS
/// (3 prioritários), algumas leituras e 1 alerta histórico.
/// Objetos anônimos permitem preencher propriedades com setter privado via HasData.
/// </summary>
internal static class DadosIniciais
{
    private static readonly DateTime Base = new(2026, 11, 18, 12, 0, 0, DateTimeKind.Utc);

    public static readonly object[] Corregos =
    [
        new
        {
            Id = 1,
            Nome = "Córrego Ponte Baixa",
            Bairro = "M'Boi Mirim",
            Latitude = -23.6905,
            Longitude = -46.7725,
            NivelAlertaCm = 120.0,
            NivelCriticoCm = 180.0,
            CriadoEm = Base.AddDays(-30)
        },
        new
        {
            Id = 2,
            Nome = "Córrego Morro do S",
            Bairro = "Campo Limpo",
            Latitude = -23.6440,
            Longitude = -46.7600,
            NivelAlertaCm = 100.0,
            NivelCriticoCm = 150.0,
            CriadoEm = Base.AddDays(-30)
        }
    ];

    public static readonly object[] Sensores =
    [
        new { Id = 1, Codigo = "ESP32-PB-01", CorregoId = 1, Ativo = true, InstaladoEm = Base.AddDays(-29), CriadoEm = Base.AddDays(-29) },
        new { Id = 2, Codigo = "ESP32-MS-01", CorregoId = 2, Ativo = true, InstaladoEm = Base.AddDays(-29), CriadoEm = Base.AddDays(-29) }
    ];

    public static readonly object[] Leituras =
    [
        Leitura(1, 1, 60.0, 5.0, NivelRisco.Normal, Base.AddHours(-6)),
        Leitura(2, 1, 95.0, 35.0, NivelRisco.Atencao, Base.AddHours(-4)),
        Leitura(3, 1, 125.0, 42.0, NivelRisco.Alerta, Base.AddHours(-3)),
        Leitura(4, 1, 110.0, 20.0, NivelRisco.Atencao, Base.AddHours(-1)),
        Leitura(5, 2, 40.0, 0.0, NivelRisco.Normal, Base.AddHours(-5)),
        Leitura(6, 2, 70.0, 12.0, NivelRisco.Normal, Base.AddHours(-3)),
        Leitura(7, 2, 80.0, 32.0, NivelRisco.Atencao, Base.AddHours(-1))
    ];

    public static readonly object[] Moradores =
    [
        Morador(1, "Maria Aparecida (fictícia)", "11900000001", "Rua Fictícia das Flores, 10 - M'Boi Mirim", 1, true, TipoNecessidade.Idoso),
        Morador(2, "João Batista (fictício)", "11900000002", "Rua Fictícia das Flores, 22 - M'Boi Mirim", 1, true, TipoNecessidade.Cadeirante),
        Morador(3, "Antônio Pereira (fictício)", "11900000003", "Travessa Exemplo, 5 - M'Boi Mirim", 1, false, TipoNecessidade.Nenhuma),
        Morador(4, "Francisca Lima (fictícia)", "11900000004", "Rua Modelo do Morro, 100 - Campo Limpo", 2, true, TipoNecessidade.Acamado),
        Morador(5, "Carlos Souza (fictício)", "11900000005", "Rua Modelo do Morro, 120 - Campo Limpo", 2, false, TipoNecessidade.Nenhuma),
        Morador(6, "Juliana Santos (fictícia)", "11900000006", "Viela Teste, 8 - Campo Limpo", 2, false, TipoNecessidade.Nenhuma)
    ];

    public static readonly object[] Alertas =
    [
        new
        {
            Id = 1,
            CorregoId = 1,
            NivelRisco = NivelRisco.Alerta,
            Mensagem = "ALERTA: Córrego Ponte Baixa atingiu 125 cm (limite de alerta 120 cm). Prepare-se para sair e acompanhe as atualizações.",
            NivelAguaCm = 125.0,
            Ativo = false,
            EncerradoEm = (DateTime?)Base.AddHours(-2),
            CriadoEm = Base.AddHours(-3)
        }
    ];

    private static object Leitura(int id, int sensorId, double nivel, double chuva, NivelRisco risco, DateTime em) => new
    {
        Id = id,
        SensorId = sensorId,
        NivelAguaCm = nivel,
        ChuvaMmHora = chuva,
        NivelRisco = risco,
        RegistradaEm = em,
        CriadoEm = em
    };

    private static object Morador(
        int id,
        string nome,
        string telefone,
        string endereco,
        int corregoId,
        bool prioritario,
        TipoNecessidade tipo) => new
    {
        Id = id,
        Nome = nome,
        Telefone = telefone,
        Endereco = endereco,
        CorregoId = corregoId,
        Prioritario = prioritario,
        TipoNecessidade = tipo,
        CriadoEm = Base.AddDays(-20)
    };
}
