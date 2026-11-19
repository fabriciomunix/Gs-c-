using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlertaCorrego.Api.Controllers;

/// <summary>Sensores ESP32 instalados nos córregos.</summary>
[ApiController]
[Route("api/sensores")]
[Produces("application/json")]
public sealed class SensoresController : ControllerBase
{
    private readonly ISensorService _sensorService;

    public SensoresController(ISensorService sensorService)
    {
        _sensorService = sensorService;
    }

    /// <summary>Lista os sensores (opcionalmente de um córrego).</summary>
    /// <param name="corregoId">Filtra pelo córrego.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SensorResponse>>> Listar(
        [FromQuery] int? corregoId,
        CancellationToken cancellationToken) =>
        Ok(await _sensorService.ListarAsync(corregoId, cancellationToken));

    /// <summary>Obtém um sensor pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorResponse>> ObterPorId(int id, CancellationToken cancellationToken) =>
        Ok(await _sensorService.ObterPorIdAsync(id, cancellationToken));

    /// <summary>Cadastra um sensor em um córrego.</summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(SensorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SensorResponse>> Criar(
        [FromBody] SensorRequest request,
        CancellationToken cancellationToken)
    {
        var criado = await _sensorService.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }
}
