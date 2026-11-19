using System.Text.Json;
using AlertaCorrego.Domain.Excecoes;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AlertaCorrego.Api.Middleware;

/// <summary>
/// Tratamento global de exceções (IExceptionHandler do .NET 8): converte as exceções
/// de domínio em respostas <see cref="ProblemDetails"/> (RFC 7807).
/// Não registra dados pessoais: apenas tipo da exceção, status e rota.
/// </summary>
public sealed class ManipuladorGlobalExcecoes : IExceptionHandler
{
    private readonly ILogger<ManipuladorGlobalExcecoes> _logger;

    public ManipuladorGlobalExcecoes(ILogger<ManipuladorGlobalExcecoes> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problema = exception switch
        {
            ValidacaoDominioException ex => CriarProblemaValidacao(ex),
            EntidadeNaoEncontradaException ex => CriarProblema(
                StatusCodes.Status404NotFound,
                "Recurso não encontrado",
                ex.Message),
            RegraNegocioException ex => CriarProblema(
                StatusCodes.Status409Conflict,
                "Regra de negócio violada",
                ex.Message),
            _ => CriarProblema(
                StatusCodes.Status500InternalServerError,
                "Erro interno",
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };

        var status = problema.Status ?? StatusCodes.Status500InternalServerError;

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Erro não tratado em {Metodo} {Rota}.",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                "Requisição {Metodo} {Rota} rejeitada com {Status} ({TipoExcecao}).",
                httpContext.Request.Method,
                httpContext.Request.Path,
                status,
                exception.GetType().Name);
        }

        problema.Instance = httpContext.Request.Path;
        problema.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync<object>(
            problema,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }

    private static ProblemDetails CriarProblema(int status, string titulo, string detalhe) => new()
    {
        Status = status,
        Title = titulo,
        Detail = detalhe
    };

    private static ValidationProblemDetails CriarProblemaValidacao(ValidacaoDominioException ex)
    {
        var campo = JsonNamingPolicy.CamelCase.ConvertName(ex.Campo);

        return new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            [campo] = [ex.Message]
        })
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Dados inválidos",
            Detail = ex.Message
        };
    }
}
