using System.Text.Json;
using System.Text.Json.Serialization;
using AlertaCorrego.Api.Middleware;
using AlertaCorrego.Application;
using AlertaCorrego.Infrastructure;
using Microsoft.OpenApi.Models;

const string PoliticaCorsApp = "AppMobile";

var builder = WebApplication.CreateBuilder(args);

// ---------- Camadas (injeção de dependência) ----------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ---------- Web API + JSON (camelCase, enums como "ATENCAO", "CRITICO"...) ----------
builder.Services
    .AddControllers()
    .AddJsonOptions(opcoes =>
    {
        opcoes.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
    });

// ---------- Tratamento global de exceções → ProblemDetails ----------
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManipuladorGlobalExcecoes>();

// ---------- CORS para o app mobile e o painel web ----------
var origensPermitidas = builder.Configuration.GetSection("Cors:OrigensPermitidas").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(opcoes =>
{
    opcoes.AddPolicy(PoliticaCorsApp, politica =>
    {
        if (origensPermitidas.Length > 0)
        {
            politica.WithOrigins(origensPermitidas);
        }
        else
        {
            politica.AllowAnyOrigin(); // ambiente acadêmico / Expo Go
        }

        politica.AllowAnyHeader().AllowAnyMethod();
    });
});

// ---------- Swagger / OpenAPI ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opcoes =>
{
    opcoes.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Alerta Córrego API",
        Version = "v1",
        Description = "API .NET 8 do CitizenTech Alerta Córrego (FIAP Global Solution): " +
                      "monitoramento do nível de córregos, classificação de risco, alertas e " +
                      "moradores prioritários. Dados de moradores são SIMULADOS."
    });

    opcoes.SupportNonNullableReferenceTypes();

    foreach (var arquivoXml in new[] { "AlertaCorrego.Api.xml", "AlertaCorrego.Application.xml" })
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, arquivoXml);
        if (File.Exists(caminho))
        {
            opcoes.IncludeXmlComments(caminho, includeControllerXmlComments: true);
        }
    }
});

var app = builder.Build();

app.UseExceptionHandler();

// Swagger habilitado em todos os ambientes para facilitar a avaliação.
app.UseSwagger();
app.UseSwaggerUI(opcoes =>
{
    opcoes.SwaggerEndpoint("/swagger/v1/swagger.json", "Alerta Córrego API v1");
    opcoes.DocumentTitle = "Alerta Córrego API";
});

app.UseCors(PoliticaCorsApp);
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Cria o banco SQLite com os dados de seed (EnsureCreated).
await app.Services.InicializarBancoDeDadosAsync();

await app.RunAsync();

/// <summary>Exposto para testes de integração (WebApplicationFactory).</summary>
public partial class Program
{
}
