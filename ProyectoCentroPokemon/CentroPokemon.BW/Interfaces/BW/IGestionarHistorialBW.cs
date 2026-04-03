namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarHistorialBW
{
    Task<object?> ObtenerHistorialPokemonAsync(int usuarioId, string rol, int pokemonId, string? tipo, string? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
    Task<byte[]> ExportarExpedienteAsync(int usuarioId, string rol, int pokemonId, string formato);
}
