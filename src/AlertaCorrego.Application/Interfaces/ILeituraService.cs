using AlertaCorrego.Application.Dtos;

namespace AlertaCorrego.Application.Interfaces;

public interface ILeituraService
{
    /// <summary>
    /// Registra a leitura, classifica o risco e, se ALERTA/CRITICO, cria o alerta
    /// e devolve os moradores prioritários a notificar.
    /// </summary>
    Task<RegistroLeituraResponse> RegistrarAsync(LeituraRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeituraResponse>> ListarAsync(int? corregoId, int limite, CancellationToken cancellationToken = default);
}
