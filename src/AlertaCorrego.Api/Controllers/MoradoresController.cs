using AlertaCorrego.Application.Dtos;
using AlertaCorrego.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlertaCorrego.Api.Controllers;

/// <summary>Moradores (dados simulados) cadastrados às margens dos córregos.</summary>
[ApiController]
[Route("api/moradores")]
[Produces("application/json")]
public sealed class MoradoresController : ControllerBase
{
    private readonly IMoradorService _moradorService;

    public MoradoresController(IMoradorService moradorService)
    {
        _moradorService = moradorService;
    }

    /// <summary>Lista os moradores (opcionalmente de um córrego).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MoradorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MoradorResponse>>> Listar(
        [FromQuery] int? corregoId,
        CancellationToken cancellationToken) =>
        Ok(await _moradorService.ListarAsync(corregoId, cancellationToken));

    /// <summary>
    /// Lista os moradores prioritários (precisam de ajuda para sair), do mais ao menos urgente.
    /// </summary>
    /// <param name="corregoId">Filtra pelo córrego.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    [HttpGet("prioritarios")]
    [ProducesResponseType(typeof(IReadOnlyList<MoradorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MoradorResponse>>> ListarPrioritarios(
        [FromQuery] int? corregoId,
        CancellationToken cancellationToken) =>
        Ok(await _moradorService.ListarPrioritariosAsync(corregoId, cancellationToken));

    /// <summary>Obtém um morador pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MoradorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MoradorResponse>> ObterPorId(int id, CancellationToken cancellationToken) =>
        Ok(await _moradorService.ObterPorIdAsync(id, cancellationToken));

    /// <summary>Cadastra um morador.</summary>
    /// <remarks>Quem informa uma necessidade (IDOSO, CADEIRANTE, ACAMADO, OUTRA) vira prioritário automaticamente.</remarks>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MoradorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MoradorResponse>> Criar(
        [FromBody] MoradorRequest request,
        CancellationToken cancellationToken)
    {
        var criado = await _moradorService.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza um morador.</summary>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MoradorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MoradorResponse>> Atualizar(
        int id,
        [FromBody] MoradorRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _moradorService.AtualizarAsync(id, request, cancellationToken));

    /// <summary>Remove um morador.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _moradorService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
