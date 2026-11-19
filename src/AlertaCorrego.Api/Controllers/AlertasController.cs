using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlertaCorrego.Api.Controllers;

/// <summary>Alertas de risco gerados a partir das leituras.</summary>
[ApiController]
[Route("api/alertas")]
[Produces("application/json")]
public sealed class AlertasController : ControllerBase
{
    private readonly IAlertaService _alertaService;

    public AlertasController(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    /// <summary>Lista os alertas, do mais recente ao mais antigo.</summary>
    /// <param name="ativos">true = só ativos; false = só encerrados; omitido = todos.</param>
    /// <param name="corregoId">Filtra pelo córrego.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AlertaResponse>>> Listar(
        [FromQuery] bool? ativos,
        [FromQuery] int? corregoId,
        CancellationToken cancellationToken) =>
        Ok(await _alertaService.ListarAsync(ativos, corregoId, cancellationToken));

    /// <summary>Encerra um alerta ativo.</summary>
    [HttpPatch("{id:int}/encerrar")]
    [ProducesResponseType(typeof(AlertaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AlertaResponse>> Encerrar(int id, CancellationToken cancellationToken) =>
        Ok(await _alertaService.EncerrarAsync(id, cancellationToken));
}
