using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlertaCorrego.Api.Controllers;

/// <summary>Córregos monitorados, status atual e estatísticas.</summary>
[ApiController]
[Route("api/corregos")]
[Produces("application/json")]
public sealed class CorregosController : ControllerBase
{
    private readonly ICorregoService _corregoService;

    public CorregosController(ICorregoService corregoService)
    {
        _corregoService = corregoService;
    }

    /// <summary>Lista todos os córregos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CorregoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CorregoResponse>>> Listar(CancellationToken cancellationToken) =>
        Ok(await _corregoService.ListarAsync(cancellationToken));

    /// <summary>Obtém um córrego pelo id.</summary>
    /// <param name="id">Id do córrego.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CorregoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CorregoResponse>> ObterPorId(int id, CancellationToken cancellationToken) =>
        Ok(await _corregoService.ObterPorIdAsync(id, cancellationToken));

    /// <summary>Cadastra um córrego.</summary>
    /// <remarks>O nível crítico deve ser maior que o nível de alerta.</remarks>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CorregoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CorregoResponse>> Criar(
        [FromBody] CorregoRequest request,
        CancellationToken cancellationToken)
    {
        var criado = await _corregoService.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza um córrego.</summary>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CorregoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CorregoResponse>> Atualizar(
        int id,
        [FromBody] CorregoRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _corregoService.AtualizarAsync(id, request, cancellationToken));

    /// <summary>Remove um córrego (e, em cascata, seus sensores, leituras, moradores e alertas).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _corregoService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Status atual: último nível, chuva, nível de risco e última leitura.</summary>
    [HttpGet("{id:int}/status")]
    [ProducesResponseType(typeof(StatusCorregoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StatusCorregoResponse>> ObterStatus(int id, CancellationToken cancellationToken) =>
        Ok(await _corregoService.ObterStatusAsync(id, cancellationToken));

    /// <summary>
    /// Estatísticas das leituras na janela informada (padrão: últimas 24 h):
    /// média, máximo e mínimo do nível, chuva máxima e contagem por nível de risco.
    /// </summary>
    /// <param name="id">Id do córrego.</param>
    /// <param name="horas">Tamanho da janela em horas (1 a 720).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [HttpGet("{id:int}/estatisticas")]
    [ProducesResponseType(typeof(EstatisticasCorregoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstatisticasCorregoResponse>> ObterEstatisticas(
        int id,
        [FromQuery] int horas = 24,
        CancellationToken cancellationToken = default) =>
        Ok(await _corregoService.ObterEstatisticasAsync(id, horas, cancellationToken));
}
