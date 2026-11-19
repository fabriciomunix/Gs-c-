using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using AlertaCorrego.Application.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace AlertaCorrego.Api.Controllers;

/// <summary>Leituras enviadas pelos sensores (nível da água e chuva).</summary>
[ApiController]
[Route("api/leituras")]
[Produces("application/json")]
public sealed class LeiturasController : ControllerBase
{
    private const int LimitePadrao = 50;

    private readonly ILeituraService _leituraService;

    public LeiturasController(ILeituraService leituraService)
    {
        _leituraService = leituraService;
    }

    /// <summary>
    /// Registra uma leitura do sensor, classifica o risco e, se ALERTA ou CRITICO,
    /// gera um alerta e devolve os moradores prioritários a notificar.
    /// </summary>
    /// <remarks>
    /// Exemplo:
    ///
    ///     POST /api/leituras
    ///     { "sensorId": 1, "nivelAguaCm": 185, "chuvaMmHora": 48 }
    ///
    /// </remarks>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegistroLeituraResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegistroLeituraResponse>> Registrar(
        [FromBody] LeituraRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await _leituraService.RegistrarAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(Listar),
            new { corregoId = resultado.Leitura.CorregoId, limite = 1 },
            resultado);
    }

    /// <summary>Lista as leituras mais recentes.</summary>
    /// <param name="corregoId">Filtra pelo córrego.</param>
    /// <param name="limite">Quantidade máxima de leituras (1 a 500, padrão 50).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LeituraResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<LeituraResponse>>> Listar(
        [FromQuery] int? corregoId,
        [FromQuery] int limite = LimitePadrao,
        CancellationToken cancellationToken = default)
    {
        if (limite is < 1 or > LeituraService.LimiteMaximo)
        {
            ModelState.AddModelError(nameof(limite), $"O limite deve estar entre 1 e {LeituraService.LimiteMaximo}.");
            return ValidationProblem(ModelState);
        }

        return Ok(await _leituraService.ListarAsync(corregoId, limite, cancellationToken));
    }
}
